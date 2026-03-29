namespace IntegrationTests.Finance.Fixtures;

/// <summary>
/// Defines a shared testing context and execution boundary for database integration tests.
/// </summary>
/// <remarks>
/// This class provides the following architectural functions within the xUnit framework:
/// <list type="bullet">
/// <item>
/// <description>
/// <b>Sequential Execution (Serialization):</b> Classes decorated with [Collection(nameof(DatabaseFixture)] 
///    will run one after another (not in parallel). This prevents "Foreign Key" or "Data Reset" 
///    errors when one test wipes a table that another test is currently reading.
/// </description>
/// </item>
/// <item>
/// <description>
/// <b>Fixture Resource Management:</b> It maps <see cref="CategoriesTestsFixture"/> and <see cref="TransactionsIntegrationTestFixture"/> 
///    to the collection, allowing them to be injected into the constructor of any test class within this collection.
/// </description>
/// </item>
/// </list>
/// </remarks>
[CollectionDefinition(nameof(DatabaseFixture))]
public abstract class DatabaseFixture : ICollectionFixture<TransactionsIntegrationTestFixture>, ICollectionFixture<CategoriesTestsFixture>, ICollectionFixture<BanksTestsFixture> { }