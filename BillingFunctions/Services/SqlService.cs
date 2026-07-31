using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using BillingFunctions.Models;


namespace BillingFunctions.Services;

public class SqlService
{
    private readonly string _connectionString;

    public SqlService(IConfiguration configuration)
    {
        _connectionString =
            configuration["SqlConnection"] ?? "";
    }

// Add a new user to the database
    public async Task AddUserAsync(
        string userName,
        string secretName)
    {
        using SqlConnection connection =
            new SqlConnection(_connectionString);

        await connection.OpenAsync();

        string query = @"
            INSERT INTO Users
            (UserName, KeyVaultSecretName)
            VALUES
            (@UserName, @SecretName)";

        SqlCommand command =
            new SqlCommand(query, connection);

        command.Parameters.AddWithValue(
            "@UserName",
            userName);

        command.Parameters.AddWithValue(
            "@SecretName",
            secretName);

        await command.ExecuteNonQueryAsync();
    }

// Get the KeyVault secret name for a given username
    public async Task<string?> GetSecretNameAsync(
    string userName)
{
    using SqlConnection connection =
        new SqlConnection(_connectionString);

    await connection.OpenAsync();

    string query =
    @"SELECT KeyVaultSecretName
      FROM Users
      WHERE UserName = @UserName";

    SqlCommand command =
        new SqlCommand(query, connection);

    command.Parameters.AddWithValue(
        "@UserName",
        userName);

    object? result =
        await command.ExecuteScalarAsync();

    return result?.ToString();
}


// Add an item to the database
public async Task AddItemAsync(
    string itemName,
    decimal price)
{
    using SqlConnection connection =
        new SqlConnection(_connectionString);

    await connection.OpenAsync();

    string query = @"
        INSERT INTO Items
        (ItemName, Price)
        VALUES
        (@ItemName, @Price)";

    SqlCommand command =
        new SqlCommand(query, connection);

    command.Parameters.AddWithValue(
        "@ItemName",
        itemName);

    command.Parameters.AddWithValue(
        "@Price",
        price);

    await command.ExecuteNonQueryAsync();
}

public async Task<List<Item>> GetItemsAsync()
{
    List<Item> items = new();

    using SqlConnection connection =
        new SqlConnection(_connectionString);

    await connection.OpenAsync();

    string query =
        @"SELECT ItemId,
                 ItemName,
                 Price
          FROM Items";

    SqlCommand command =
        new SqlCommand(query, connection);

    SqlDataReader reader =
        await command.ExecuteReaderAsync();

    while (await reader.ReadAsync())
    {
        items.Add(new Item
        {
            ItemId = Convert.ToInt32(
                reader["ItemId"]),

            ItemName = reader["ItemName"]
                .ToString()!,

            Price = Convert.ToDecimal(
                reader["Price"])
        });
    }

    return items;
}

// Get an item by its ID
public async Task<Item?> GetItemByIdAsync(
    int itemId)
{
    using SqlConnection connection =
        new SqlConnection(_connectionString);

    await connection.OpenAsync();

    string query =
    @"SELECT ItemId,
             ItemName,
             Price
      FROM Items
      WHERE ItemId = @ItemId";

    SqlCommand command =
        new SqlCommand(query, connection);

    command.Parameters.AddWithValue(
        "@ItemId",
        itemId);

    SqlDataReader reader =
        await command.ExecuteReaderAsync();

    if (await reader.ReadAsync())
    {
        return new Item
        {
            ItemId =
                Convert.ToInt32(reader["ItemId"]),

            ItemName =
                reader["ItemName"].ToString()!,

            Price =
                Convert.ToDecimal(reader["Price"])
        };
    }

    return null;
}

// Save a bill to the database and return the generated BillId
public async Task<int> SaveBillAsync(
    string customerName,
    string customerPhone,
    decimal totalAmount)
{
    using SqlConnection connection =
        new SqlConnection(_connectionString);

    await connection.OpenAsync();

    string query = @"
    INSERT INTO Bills
    (
        CustomerName,
        CustomerPhone,
        TotalAmount
    )
    OUTPUT INSERTED.BillId
    VALUES
    (
        @CustomerName,
        @CustomerPhone,
        @TotalAmount
    )";

    SqlCommand command =
        new SqlCommand(query, connection);

    command.Parameters.AddWithValue(
        "@CustomerName",
        customerName);

    command.Parameters.AddWithValue(
        "@CustomerPhone",
        customerPhone);

    command.Parameters.AddWithValue(
        "@TotalAmount",
        totalAmount);

    int billId =
        Convert.ToInt32(
            await command.ExecuteScalarAsync());

    return billId;
}

// Save a bill item to the database
public async Task SaveBillItemAsync(
    int billId,
    int itemId,
    int quantity,
    decimal lineTotal)
{
    using SqlConnection connection =
        new SqlConnection(_connectionString);

    await connection.OpenAsync();

    string query = @"
    INSERT INTO BillItems
    (
        BillId,
        ItemId,
        Quantity,
        LineTotal
    )
    VALUES
    (
        @BillId,
        @ItemId,
        @Quantity,
        @LineTotal
    )";

    SqlCommand command =
        new SqlCommand(query, connection);

    command.Parameters.AddWithValue(
        "@BillId",
        billId);

    command.Parameters.AddWithValue(
        "@ItemId",
        itemId);

    command.Parameters.AddWithValue(
        "@Quantity",
        quantity);

    command.Parameters.AddWithValue(
        "@LineTotal",
        lineTotal);

    await command.ExecuteNonQueryAsync();
}

}