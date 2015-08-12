﻿using ExpressMapper.Tests.Model.Enums;
using ExpressMapper.Tests.Model.Generator;
using ExpressMapper.Tests.Model.Models;
using ExpressMapper.Tests.Model.Models.Structs;
using ExpressMapper.Tests.Model.ViewModels;
using ExpressMapper.Tests.Model.ViewModels.Structs;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpressMapper.Tests
{
    [TestFixture]
    public class BasicTests : BaseTestClass
    {
        [Test]
        public void RecursiveCompilationDirectCollectionTest()
        {
            Mapper.Register<Employee, EmployeeViewModel>();
            Mapper.Compile();

            var srcAndDest = Functional.RecursiveCompilationDirectCollectionTestMap();

            var bvm = Mapper.Map<Employee, EmployeeViewModel>(srcAndDest.Key);

            Assert.AreEqual(bvm, srcAndDest.Value);
        }

        [Test]
        public void RecursiveCompilationAssociationTest()
        {
            Mapper.Register<Booking, BookingViewModel>();
            Mapper.Register<Composition, CompositionViewModel>();
            Mapper.Compile();

            var srcAndDest = Functional.RecursiveCompilationAssociationTestMap();

            var bvm = Mapper.Map<Booking, BookingViewModel>(srcAndDest.Key);

            Assert.AreEqual(bvm, srcAndDest.Value);
        }

        [Test]
        public void RecursiveCompilationCollectionTest()
        {
            Mapper.Register<Engine, EngineViewModel>();
            Mapper.Register<Cylinder, CylinderViewModel>();
            Mapper.Compile();

            var srcAndDest = Functional.RecursiveCompilationCollectionTestMap();
            var engineViewModel = Mapper.Map<Engine, EngineViewModel>(srcAndDest.Key);
            Assert.AreEqual(engineViewModel, srcAndDest.Value);
        }


        [Test]
        public void CompilelessMap()
        {
            Mapper.Register<TestModel, TestViewModel>();
            Mapper.Register<Size, SizeViewModel>();
            Mapper.Register<Country, CountryViewModel>();

            var test = Functional.AutoMemberMap();

            var testViewModel = Mapper.Map<TestModel, TestViewModel>(test.Key);

            Assert.AreEqual(testViewModel, test.Value);
        }

        [Test]
        public void AutoMemberMap()
        {
            Mapper.Register<TestModel, TestViewModel>();
            Mapper.Register<Size, SizeViewModel>();
            Mapper.Register<Country, CountryViewModel>();
            Mapper.Compile();

            var test = Functional.AutoMemberMap();

            var testViewModel = Mapper.Map<TestModel, TestViewModel>(test.Key);

            Assert.AreEqual(testViewModel, test.Value);
        }

        [Test]
        public void ManualPrimitiveMemberMap()
        {
            Mapper.Register<Size, SizeViewModel>()
                .Member(src => src.Name, dest => string.Format("Full - {0} - Size", dest.Alias))
                .Member(src => src.SortOrder, dest => dest.Id.GetHashCode())
                .Member(src => src.Nullable, dest => dest.Nullable)
                .Member(src => src.NotNullable, dest => dest.NotNullable)
                .Member(src => src.BoolValue, dest => dest.BoolValue);
            Mapper.Compile();

            var sizeResult = Functional.ManualPrimitiveMemberMap();

            var result = Mapper.Map<Size, SizeViewModel>(sizeResult.Key);

            Assert.AreEqual(result, sizeResult.Value);
        }

        [Test]
        public void ManualConstantMemberMap()
        {
            Mapper.Register<Size, SizeViewModel>()
                .Value(src => src.SortOrder, 123)
                .Value(src => src.BoolValue, true);
            Mapper.Compile();

            var sizeResult = Functional.ManualPrimitiveMemberMap();

            var result = Mapper.Map<Size, SizeViewModel>(sizeResult.Key);

            Assert.AreEqual(result.SortOrder, 123);
            Assert.AreEqual(result.BoolValue, true);
        }

        [Test]
        public void ManualNestedNotNullMemberMap()
        {
            Mapper.Register<Trip, TripViewModel>()
                .Member(src => src.Name, dest => dest.Category.Name)
                .Ignore(x => x.Category);
            Mapper.Compile();

            var source = new Trip()
            {
                Category = new CategoryTrip()
                {
                    Name = "TestCat123"
                },
                Name = "abc"
            };

            var result = Mapper.Map<Trip, TripViewModel>(source);

            Assert.IsNull(result.Category);
            Assert.AreEqual(result.Name, "TestCat123");
        }

        [Test]
        public void ManualNestedNullMemberMap()
        {
            Mapper.Register<Trip, TripViewModel>()
                .Member(src => src.Name, dest => dest.Category.Name)
                .Ignore(x => x.Category);
            Mapper.Compile();

            var source = new Trip()
            {
                Name = "abc"
            };

            var result = Mapper.Map<Trip, TripViewModel>(source);

            Assert.IsNull(result.Category);
            Assert.IsNull(result.Name);
        }

        [Test]
        public void InstantiateMap()
        {
            Mapper.Register<Size, SizeViewModel>()
                .Instantiate(src => new SizeViewModel(s => string.Format("{0} - Full name - {1}", src.Id, s)));
            Mapper.Compile();

            var sizeResult = Functional.InstantiateMap();

            var result = Mapper.Map<Size, SizeViewModel>(sizeResult.Key);

            Assert.AreEqual(result, sizeResult.Value);
        }

        [Test]
        public void IgnoreMap()
        {
            Mapper.Register<Size, SizeViewModel>()
                .Ignore(dest => dest.Name);
            Mapper.Compile();

            var sizeResult = Functional.IgnoreMap();
            var result = Mapper.Map<Size, SizeViewModel>(sizeResult.Key);
            Assert.AreEqual(result, sizeResult.Value);
        }

        [Test]
        public void BeforeMap()
        {
            Mapper.Register<Size, SizeViewModel>()
                .Before((src, dest) => dest.Name = src.Name)
                .Ignore(dest => dest.Name);
            Mapper.Compile();

            var sizeResult = Functional.BeforeMap();
            var result = Mapper.Map<Size, SizeViewModel>(sizeResult.Key);
            Assert.AreEqual(result, sizeResult.Value);
        }

        [Test]
        public void AfterMap()
        {
            Mapper.Register<Size, SizeViewModel>()
                .After((src, dest) => dest.Name = "OVERRIDE BY AFTER MAP");
            Mapper.Compile();
            var sizeResult = Functional.AfterMap();
            var result = Mapper.Map<Size, SizeViewModel>(sizeResult.Key);
            Assert.AreEqual(result, sizeResult.Value);
        }


        [Test]
        public void CustomMapWithSupportedCollectionMaps()
        {
            Mapper.RegisterCustom<Size, SizeViewModel, SizeMapper>();
            Mapper.Compile();
            var sizeResult = Functional.CustomMap();
            var result = Mapper.Map<Size, SizeViewModel>(sizeResult.Key);
            Assert.AreEqual(result, sizeResult.Value);
        }

        [Test]
        public void CustomMapWithSupportedNestedCollectionMaps()
        {
            Mapper.Register<TestModel, TestViewModel>();
            Mapper.Register<Country, CountryViewModel>();
            Mapper.RegisterCustom<Size, SizeViewModel, SizeMapper>();
            Mapper.Compile();
            var sizeResult = Functional.CustomNestedCollectionMap();
            var result = Mapper.Map<TestModel, TestViewModel>(sizeResult.Key);
            Assert.AreEqual(result, sizeResult.Value);
        }

        [Test]
        public void CustomMapListWithSupportedNestedCollectionMaps()
        {
            Mapper.Register<TestModel, TestViewModel>();
            Mapper.Register<Country, CountryViewModel>();
            Mapper.RegisterCustom<List<Size>, List<SizeViewModel>, SizeListMapper>();
            Mapper.Compile();
            var sizeResult = Functional.CustomNestedCollectionMap();
            var result = Mapper.Map<TestModel, TestViewModel>(sizeResult.Key);
            Assert.AreEqual(result, sizeResult.Value);
        }

        [Test]
        public void ExistingDestCustomMapWithSupportedNestedCollectionMaps()
        {
            Mapper.Register<TestModel, TestViewModel>();
            Mapper.Register<Country, CountryViewModel>();
            Mapper.RegisterCustom<Size, SizeViewModel, SizeMapper>();
            Mapper.Compile();
            var sizeResult = Functional.CustomNestedCollectionMap();
            var result = Mapper.Map(sizeResult.Key, sizeResult.Value);
            Assert.AreEqual(result, sizeResult.Value);
        }

        [Test]
        public void ExistingDestCustomMapListWithSupportedNestedCollectionMaps()
        {
            Mapper.Register<TestModel, TestViewModel>();
            Mapper.Register<Country, CountryViewModel>();
            Mapper.RegisterCustom<List<Size>, List<SizeViewModel>, SizeListMapper>();
            Mapper.Compile();
            var sizeResult = Functional.CustomNestedCollectionMap();
            var result = Mapper.Map(sizeResult.Key, sizeResult.Value);
            Assert.AreEqual(result, sizeResult.Value);
        }

        [Test]
        public void NullPropertyAndNullCollectionPropertyMaps()
        {
            Mapper.Register<TestModel, TestViewModel>();
            Mapper.Register<Country, CountryViewModel>();
            Mapper.Register<Size, SizeViewModel>();

            Mapper.Compile();
            var sizeResult = Functional.NullPropertyAndNullCollectionMap();
            var result = Mapper.Map<TestModel, TestViewModel>(sizeResult.Key);
            Assert.AreEqual(result, sizeResult.Value);
        }

        [Test]
        public void OnlyGetPropertyMaps()
        {
            Mapper.Register<Supplier, SupplierViewModel>();
            Mapper.Register<Size, SizeViewModel>();
            Mapper.Compile();
            var supplierResult = Functional.GetPropertyMaps();
            var result = Mapper.Map<Supplier, SupplierViewModel>(supplierResult.Key);
            Assert.AreEqual(result, supplierResult.Value);
        }

        [Test]
        public void OnlyGetWithManualPropertyMaps()
        {
            Mapper.Register<Supplier, SupplierViewModel>()
                .Member(dest => dest.Rank, src => src.Rank);
            Mapper.Register<Size, SizeViewModel>();
            Mapper.Compile();
            var supplierResult = Functional.GetPropertyMaps();
            var result = Mapper.Map<Supplier, SupplierViewModel>(supplierResult.Key);
            Assert.AreEqual(result, supplierResult.Value);
        }

        [Test]
        public void CustomMap()
        {
            Mapper.RegisterCustom<GenderTypes, string>(g => g.ToString());
            Mapper.Compile();

            var result = Mapper.Map<GenderTypes, string>(GenderTypes.Men);
            Assert.AreEqual(result, GenderTypes.Men.ToString());
        }

        [Test]
        public void AutoMemberStructMap()
        {
            Mapper.Register<Item, ItemViewModel>();
            Mapper.Compile();
            var testData = Functional.AutoMemberStructMap();

            var result = Mapper.Map<Item, ItemViewModel>(testData.Key);
            Assert.AreEqual(result, testData.Value);
        }

        [Test]
        public void StructWithCollectionMap()
        {
            Mapper.Register<Feature, FeatureViewModel>();
            Mapper.Register<Item, ItemViewModel>()
                .Member(dest => dest.FeatureList, src => src.Features);
            Mapper.Compile();
            var testData = Functional.StructWithCollectionMap();

            var result = Mapper.Map<Item, ItemViewModel>(testData.Key);
            Assert.AreEqual(result, testData.Value);
        }

        [Test]
        public void ComplexMap()
        {
            Mapper.Register<FashionProduct, FashionProductViewModel>()
                .Function(dest => dest.OptionalGender, src =>
                {
                    GenderTypes? optionalGender;
                    switch (src.Gender)
                    {
                        case GenderTypes.Unisex:
                            optionalGender = null;
                            break;
                        default:
                            optionalGender = src.Gender;
                            break;
                    }
                    return optionalGender;
                });
            Mapper.Register<ProductOption, ProductOptionViewModel>();
            Mapper.Register<Size, SizeViewModel>();
            Mapper.Register<Feature, FeatureViewModel>();
            Mapper.Register<City, CityViewModel>()
                .Member(dest => dest.FeaturesList, src => src.Features);
            Mapper.Register<Supplier, SupplierViewModel>();
            Mapper.Register<Brand, BrandViewModel>();

            Mapper.Compile();
            var testData = Functional.ComplexMap();

            var result = Mapper.Map<FashionProduct, FashionProductViewModel>(testData.Key);
            var valid = result.Equals(testData.Value);
            Assert.IsTrue(valid);
        }

        [Test]
        public void ListToArray()
        {
            Mapper.Register<TestCollection, TestCollectionViewModel>();
            Mapper.Register<TestItem, TestItemViewModel>()
                .Member(dest => dest.Array, src => src.List)
                .Ignore(dest => dest.Collection)
                .Ignore(dest => dest.Enumerable)
                .Ignore(dest => dest.List)
                .Ignore(dest => dest.Queryable);

            Mapper.Compile();

            var typeCollTest = Functional.CollectionTypeMap();
            var result = Mapper.Map<TestItem, TestItemViewModel>(typeCollTest.Key);
            Assert.AreEqual(result.Array.Length, typeCollTest.Key.List.Count);
        }

        [Test]
        public void ListNullToArray()
        {
            Mapper.Register<TestCollection, TestCollectionViewModel>();
            Mapper.Register<TestItem, TestItemViewModel>()
                .Member(dest => dest.Array, src => src.List)
                .Ignore(dest => dest.Collection)
                .Ignore(dest => dest.Enumerable)
                .Ignore(dest => dest.List)
                .Ignore(dest => dest.Queryable);

            Mapper.Compile();

            var typeCollTest = new TestItem()
            {
                List = null
            };

            var result = Mapper.Map<TestItem, TestItemViewModel>(typeCollTest);

            Assert.IsNull(result.Array);
        }

        [Test]
        public void ListToQueriable()
        {
            Mapper.Register<TestCollection, TestCollectionViewModel>();
            Mapper.Register<TestItem, TestItemViewModel>()
                .Member(dest => dest.Queryable, src => src.List)
                .Ignore(dest => dest.Collection)
                .Ignore(dest => dest.List)
                .Ignore(dest => dest.Array)
                .Ignore(dest => dest.Enumerable);

            Mapper.Compile();

            var typeCollTest = Functional.CollectionTypeMap();
            var result = Mapper.Map<TestItem, TestItemViewModel>(typeCollTest.Key);
            Assert.AreEqual(result.Queryable.Count(), typeCollTest.Key.List.Count());
        }

        [Test]
        public void EnumerableToQueriable()
        {
            Mapper.Register<TestCollection, TestCollectionViewModel>();
            Mapper.Register<TestItem, TestItemViewModel>()
                .Member(dest => dest.Queryable, src => src.Enumerable)
                .Ignore(dest => dest.Collection)
                .Ignore(dest => dest.List)
                .Ignore(dest => dest.Array)
                .Ignore(dest => dest.Enumerable);

            Mapper.Compile();

            var typeCollTest = Functional.CollectionTypeMap();
            var result = Mapper.Map<TestItem, TestItemViewModel>(typeCollTest.Key);
            Assert.AreEqual(result.Queryable.Count(), typeCollTest.Key.Enumerable.Count());
        }

        [Test]
        public void QueryableToArray()
        {
            Mapper.Register<TestCollection, TestCollectionViewModel>();
            Mapper.Register<TestItem, TestItemViewModel>()
                .Member(dest => dest.Array, src => src.Queryable)
                .Ignore(dest => dest.Collection)
                .Ignore(dest => dest.List)
                .Ignore(dest => dest.Queryable)
                .Ignore(dest => dest.Enumerable);

            Mapper.Compile();

            var typeCollTest = Functional.CollectionTypeMap();
            var result = Mapper.Map<TestItem, TestItemViewModel>(typeCollTest.Key);
            Assert.AreEqual(result.Array.Count(), typeCollTest.Key.Queryable.Count());
        }

        [Test]
        public void NonGenericSimpleMap()
        {
            Mapper.Register<TestModel, TestViewModel>();
            Mapper.Register<Size, SizeViewModel>();
            Mapper.Register<Country, CountryViewModel>();
            Mapper.Compile();

            var test = Functional.AutoMemberMap();

            var testViewModel = Mapper.Map(test.Key, typeof(TestModel), typeof(TestViewModel)) as TestViewModel;

            Assert.AreEqual(testViewModel, test.Value);
        }

        [Test]
        public void CustomMapNonGeneric()
        {
            Mapper.RegisterCustom<Size, SizeViewModel, SizeMapper>();
            Mapper.Compile();
            var sizeResult = Functional.CustomMap();
            var result = Mapper.Map(sizeResult.Key, typeof(Size), typeof(SizeViewModel));
            Assert.AreEqual(result, sizeResult.Value);
        }

        [Test]
        public void AccessSourceNestedProperty()
        {
            Mapper.Register<TestModel, TestViewModel>()
                .Member(dest => dest.Name, src => string.Format("Test - {0} - and date: {1} plus {2}", src.Country.Name, DateTime.Now, src.Country.Code));
            Mapper.Register<Country, CountryViewModel>();
            Mapper.Register<Size, SizeViewModel>();

            Mapper.Compile();

            var sizeResult = Functional.AccessSourceNestedProperty();

            var result = Mapper.Map<TestModel, TestViewModel>(sizeResult.Key);
            Assert.AreEqual(result, sizeResult.Value);
        }

        [Test]
        public void AccessSourceManyNestedProperties()
        {
            Mapper.Register<Trip, TripViewModel>()
                .Member(dest => dest.Name, src => string.Format("Type: {0}, Catalog: {1}, Category: {2}", src.Category.Catalog.TripType.Name, src.Category.Catalog.Name, src.Category.Name))
                .Ignore(dest => dest.Category);

            Mapper.Compile();

            var sizeResult = Functional.AccessSourceManyNestedProperties();

            var result = Mapper.Map<Trip, TripViewModel>(sizeResult.Key);
            Assert.AreEqual(result, sizeResult.Value);
        }

        [Test]
        public void ExistingDestinationSimple()
        {
            Mapper.Register<TestModel, TestViewModel>();
            Mapper.Register<Country, CountryViewModel>();
            Mapper.Register<Size, SizeViewModel>();

            var sizeResult = Functional.ExistingDestinationSimpleMap();

            var testObjHash = sizeResult.Value.GetHashCode();
            var countryHash = sizeResult.Value.Country.GetHashCode();
            var sizesHash = sizeResult.Value.Sizes.GetHashCode();
            var sizeHashesList = new List<int>(sizeResult.Value.Sizes.Count);
            sizeHashesList.AddRange(sizeResult.Value.Sizes.Select(size => size.GetHashCode()));


            var result = Mapper.Map<TestModel, TestViewModel>(sizeResult.Key, sizeResult.Value);
            Assert.AreEqual(result, sizeResult.Value);
            Assert.AreEqual(result.GetHashCode(), testObjHash);
            Assert.AreEqual(result.Country.GetHashCode(), countryHash);
            Assert.AreEqual(result.Sizes.GetHashCode(), sizesHash);

            for (var i = 0; i < result.Sizes.Count; i++)
            {
                Assert.AreEqual(result.Sizes[i].GetHashCode(), sizeHashesList[i]);
            }
        }

        [Test]
        public void ExistingDestCollEquals()
        {
            Mapper.Register<TestCollection, TestCollectionViewModel>();
            Mapper.Register<TestItem, TestItemViewModel>()
                .Member(dest => dest.Array, src => src.Queryable)
                .Ignore(dest => dest.Queryable);

            Mapper.Compile();
            var testResult = Functional.ExistingDestCollEquals();

            var testItemHash = testResult.Value.GetHashCode();
            var arrayHash = testResult.Value.Array.GetHashCode();
            var testArr = new List<int>(testResult.Value.Array.Length);
            testArr.AddRange(testResult.Value.Array.Select(tc => tc.GetHashCode()));


            var result = Mapper.Map(testResult.Key, testResult.Value);
            Assert.AreEqual(result, testResult.Value);
            Assert.AreEqual(result.GetHashCode(), testItemHash);
            Assert.AreEqual(result.Array.GetHashCode(), arrayHash);

            for (var i = 0; i < result.Array.Length; i++)
            {
                Assert.AreEqual(result.Array[i].GetHashCode(), testArr[i]);
            }
        }

        [Test]
        public void ExistingDestCollEqualsWithNullElement()
        {
            Mapper.Register<TestCollection, TestCollectionViewModel>();
            Mapper.Register<TestItem, TestItemViewModel>()
                .Member(dest => dest.Array, src => src.Queryable)
                .Ignore(dest => dest.Queryable);

            Mapper.Compile();
            var testResult = Functional.ExistingDestCollEqualsWithNullElement();

            var testItemHash = testResult.Value.GetHashCode();
            var arrayHash = testResult.Value.Array.GetHashCode();
            var testArr = new List<int?>(testResult.Value.Array.Length);
            testArr.AddRange(testResult.Value.Array.Select(tc => tc == null ? (int?)null : tc.GetHashCode()));

            var result = Mapper.Map(testResult.Key, testResult.Value);
            Assert.AreEqual(result, testResult.Value);
            Assert.AreEqual(result.GetHashCode(), testItemHash);
            Assert.AreEqual(result.Array.GetHashCode(), arrayHash);

            for (var i = 0; i < result.Array.Length; i++)
            {
                if (i == 3)
                {
                    Assert.AreEqual(null, result.Array[i]);
                    Assert.AreEqual(null, testArr[i]);
                }
                else
                {
                    Assert.AreEqual(result.Array[i].GetHashCode(), testArr[i]);
                }
            }
        }

        [Test]
        public void ExistingSrcCollGreater()
        {
            Mapper.Register<TestCollection, TestCollectionViewModel>();
            Mapper.Register<TestItem, TestItemViewModel>()
                .Member(dest => dest.Collection, src => src.Array)
                .Ignore(dest => dest.Array);

            Mapper.Compile();
            var testResult = Functional.ExistingDestSrcCollGreater();

            var testItemHash = testResult.Value.GetHashCode();
            var collectionHash = testResult.Value.Collection.GetHashCode();
            var testColl = new List<int>(testResult.Value.Collection.Count);
            testColl.AddRange(testResult.Value.Collection.Select(tc => tc.GetHashCode()));

            var result = Mapper.Map(testResult.Key, testResult.Value);
            Assert.AreEqual(result, testResult.Value);
            Assert.AreEqual(result.GetHashCode(), testItemHash);
            Assert.AreEqual(result.Collection.GetHashCode(), collectionHash);

            for (var i = 0; i < 6; i++)
            {
                Assert.AreEqual(result.Collection.ElementAt(i).GetHashCode(), testColl[i]);
            }
        }

        [Test]
        public void ExistingDestDestCollGreater()
        {
            Mapper.Register<TestCollection, TestCollectionViewModel>();
            Mapper.Register<TestItem, TestItemViewModel>()
                .Member(dest => dest.List, src => src.Collection)
                .Ignore(dest => dest.Collection);

            Mapper.Compile();
            var testResult = Functional.ExistingDestCollGreater();

            var testItemHash = testResult.Value.GetHashCode();
            var listHash = testResult.Value.List.GetHashCode();
            var testList = new List<int>(testResult.Value.List.Count);
            testList.AddRange(testResult.Value.List.Select(tc => tc.GetHashCode()));


            var result = Mapper.Map(testResult.Key, testResult.Value);
            Assert.AreEqual(result, testResult.Value);
            Assert.AreEqual(result.GetHashCode(), testItemHash);
            Assert.AreEqual(result.List.GetHashCode(), listHash);

            for (var i = 0; i < 6; i++)
            {
                Assert.AreEqual(result.List.ElementAt(i).GetHashCode(), testList[i + 4]);
            }
        }

        [Test]
        public void ExistingDestCollNotEqual()
        {
            Mapper.Register<TestCollection, TestCollectionViewModel>();
            Mapper.Register<TestItem, TestItemViewModel>()
                .Member(dest => dest.Array, src => src.Collection)
                .Ignore(dest => dest.Collection);

            Mapper.Compile();
            var testResult = Functional.ExistingDestCollNotEqual();

            var testItemHash = testResult.Value.GetHashCode();
            var listHash = testResult.Value.Array.GetHashCode();
            var testList = new List<int>(testResult.Value.Array.Length);
            testList.AddRange(testResult.Value.Array.Select(tc => tc.GetHashCode()));

            var result = Mapper.Map(testResult.Key, testResult.Value);

            Assert.AreEqual(result, testResult.Value);
            Assert.AreEqual(result.GetHashCode(), testItemHash);
            Assert.AreNotEqual(result.Array.GetHashCode(), listHash);

            for (var i = 0; i < 6; i++)
            {
                Assert.AreEqual(result.Array[i].GetHashCode(), testList[i]);
            }
        }

        [Test]
        public void ExistingDestinationComplex()
        {
            Mapper.Register<ItemModel, ItemModelViewModel>();
            Mapper.Register<SubItem, SubItemViewModel>();
            Mapper.Register<Unit, UnitViewModel>();
            Mapper.Register<SubUnit, SubUnitViewModel>();
            Mapper.Compile();

            var sizeResult = Functional.ExistingDestinationComplex();

            var itemModelHash = sizeResult.Value.GetHashCode();
            var itemModelSubItemsHash = sizeResult.Value.SubItems.GetHashCode();

            var subItemsHashes = new List<int>(sizeResult.Value.SubItems.Length);
            var subItemUnitsCollHashes = new Dictionary<int, int>();
            var subItemUnitsHashes = new Dictionary<int, List<int>>();
            var subItemUnitSubUnitCollHashes = new Dictionary<int, List<int>>();
            var subItemUnitSubUnitsHashes = new Dictionary<int, Dictionary<int, List<int>>>();

            foreach (var subItem in sizeResult.Value.SubItems)
            {
                var sbHash = subItem.GetHashCode();
                subItemsHashes.Add(sbHash);
                subItemUnitsCollHashes.Add(sbHash, subItem.Units.GetHashCode());
                subItemUnitsHashes.Add(sbHash, new List<int>());
                subItemUnitSubUnitCollHashes.Add(sbHash, new List<int>());
                subItemUnitSubUnitsHashes.Add(sbHash, new Dictionary<int, List<int>>());

                foreach (var unit in subItem.Units.Skip(1))
                {
                    subItemUnitsHashes[sbHash].Add(unit.GetHashCode());
                    subItemUnitSubUnitCollHashes[sbHash].Add(unit.SubUnits.GetHashCode());
                    subItemUnitSubUnitsHashes[sbHash][unit.GetHashCode()] = new List<int>();

                    foreach (var subUnit in unit.SubUnits)
                    {
                        subItemUnitSubUnitsHashes[sbHash][unit.GetHashCode()].Add(subUnit.GetHashCode());
                    }
                }
            }

            var result = Mapper.Map(sizeResult.Key, sizeResult.Value);

            Assert.AreEqual(result, sizeResult.Value);
            Assert.AreEqual(result.GetHashCode(), itemModelHash);
            Assert.AreEqual(result.SubItems.GetHashCode(), itemModelSubItemsHash);
            for (var i = 0; i < result.SubItems.Length; i++)
            {
                Assert.AreEqual(result.SubItems[i].GetHashCode(), subItemsHashes[i]);
                Assert.AreEqual(result.SubItems[i].Units.GetHashCode(), subItemUnitsCollHashes[result.SubItems[i].GetHashCode()]);

                for (var j = 0; j < 4; j++)
                {
                    Assert.AreEqual(result.SubItems[i].Units[j].GetHashCode(), subItemUnitsHashes[result.SubItems[i].GetHashCode()][j]);
                    Assert.AreEqual(result.SubItems[i].Units[j].SubUnits.GetHashCode(), subItemUnitSubUnitCollHashes[result.SubItems[i].GetHashCode()][j]);
                    for (var k = 0; k < 3; k++)
                    {
                        Assert.AreEqual(result.SubItems[i].Units[j].SubUnits[k].GetHashCode(), subItemUnitSubUnitsHashes[result.SubItems[i].GetHashCode()][result.SubItems[i].Units[j].GetHashCode()][k]);
                    }
                }
            }
        }

        [Test]
        public void ExistingDestinationMedium()
        {
            Mapper.Register<TripType, TripTypeViewModel>();
            Mapper.Register<TripCatalog, TripCatalogViewModel>();
            Mapper.Register<CategoryTrip, CategoryTripViewModel>();
            Mapper.Register<Trip, TripViewModel>();

            Mapper.Compile();
            var tripResult = Functional.ExistingDestinationMediumMap();

            var tripHash = tripResult.Value.GetHashCode();
            var tripCatHash = tripResult.Value.Category.GetHashCode();
            var tripCatCtlHash = tripResult.Value.Category.Catalog.GetHashCode();
            var tripCatCtlTypeHash = tripResult.Value.Category.Catalog.TripType.GetHashCode();


            var result = Mapper.Map<Trip, TripViewModel>(tripResult.Key, tripResult.Value);
            Assert.AreEqual(result, tripResult.Value);
            Assert.AreEqual(result.GetHashCode(), tripHash);
            Assert.AreEqual(result.Category.GetHashCode(), tripCatHash);
            Assert.AreEqual(result.Category.Catalog.GetHashCode(), tripCatCtlHash);
            Assert.AreEqual(result.Category.Catalog.TripType.GetHashCode(), tripCatCtlTypeHash);
        }

        [Test]
        public void EnumMap()
        {
            Mapper.Register<TestModel, TestViewModel>()
                .Ignore(x => x.Country)
                .Ignore(x => x.Sizes)
                .Member(x => x.GenderIndex, x => x.NullableGender);
            Mapper.Compile();

            var test = new TestModel()
            {
                Gender = GenderTypes.Men.ToString(),
                NullableGender = GenderTypes.Women
            };

            var testViewModel = Mapper.Map<TestModel, TestViewModel>(test);

            Assert.AreEqual(GenderTypes.Men, testViewModel.Gender);
            Assert.AreEqual(GenderTypes.Women.ToString(), testViewModel.NullableGender);
            Assert.AreEqual((int)GenderTypes.Women, testViewModel.GenderIndex);
        }

        [Test]
        public void ConvertibleMap()
        {
            Mapper.Register<TestModel, TestViewModel>()
                .Ignore(x => x.Country)
                .Ignore(x => x.Sizes)
                .Member(x => x.GenderIndex, x => x.BoolValue)
                .Member(x => x.NotNullable, x => x.Height);
            Mapper.Compile();

            var test = new TestModel()
            {
                BoolValue = true,
                Height = 123
            };

            var testViewModel = Mapper.Map<TestModel, TestViewModel>(test);

            Assert.AreEqual(1, testViewModel.GenderIndex);
            Assert.AreEqual(123, testViewModel.NotNullable);
        }
    }
}
