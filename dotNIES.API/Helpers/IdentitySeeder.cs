using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;

namespace dotNIES.API.Helpers;

public class IdentitySeeder
{
    private readonly IDbConnection _connection;

    public IdentitySeeder(string connectionString)
    {
        _connection = new SqlConnection(connectionString);
    }

    // Hash een wachtwoord met PBKDF2
    private async Task<string> HashPassword(string username, string password)
    {
        var user = new IdentityUser { UserName = username };
        var hasher = new PasswordHasher<IdentityUser>();

        var hashedPassword = hasher.HashPassword(user, password);

        return hashedPassword;
    }

    public async Task SeedAccounts()
    {
        // Controleer of tabellen bestaan
        if (!await TablesExist())
        {
            throw new Exception("De benodigde Identity-tabellen bestaan niet. Maak ze eerst aan.");
        }

        // Admin account
        await SeedUser(
            userName: "peter",
            email: "peter@dotnies.be",
            password: "test",
            role: "Admin");

        // Normale gebruiker
        await SeedUser(
            userName: "mobuser1",
            email: "peter@dotnies.be",
            password: "mobuser1",
            role: "User");

        // Normale gebruiker
        await SeedUser(
            userName: "mobuser2",
            email: "peter@dotnies.be",
            password: "mobuser2",
            role: "User");

        Console.WriteLine("Gebruikers succesvol toegevoegd!");
    }

    private async Task<bool> TablesExist()
    {
        const string sql = @"
                SELECT COUNT(*) 
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_NAME IN ('AspNetUsers', 'AspNetRoles', 'AspNetUserRoles')";

        int count = await _connection.ExecuteScalarAsync<int>(sql);
        return count == 3;
    }

    private async Task SeedUser(string userName, string email, string password, string role)
    {
        // Controleer of gebruiker al bestaat
        var existingUser = await _connection.QueryFirstOrDefaultAsync<string>(
            "SELECT Id FROM AspNetUsers WHERE NormalizedUserName = @NormalizedUserName",
            new { NormalizedUserName = userName.ToUpper() });

        if (existingUser != null)
        {
            Console.WriteLine($"Gebruiker '{userName}' bestaat al, overslaan...");
            return;
        }

        // Gebruiker toevoegen
        string userId = Guid.NewGuid().ToString();
        string passwordHash = await HashPassword(userName, password);

        await _connection.ExecuteAsync(@"
                INSERT INTO AspNetUsers (
                    Id, UserName, NormalizedUserName, Email, NormalizedEmail,
                    EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp,
                    PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled,
                    LockoutEnd, LockoutEnabled, AccessFailedCount
                )
                VALUES (
                    @Id, @UserName, @NormalizedUserName, @Email, @NormalizedEmail,
                    @EmailConfirmed, @PasswordHash, @SecurityStamp, @ConcurrencyStamp,
                    @PhoneNumber, @PhoneNumberConfirmed, @TwoFactorEnabled,
                    @LockoutEnd, @LockoutEnabled, @AccessFailedCount
                )",
            new
            {
                Id = userId,
                UserName = userName,
                NormalizedUserName = userName.ToUpper(),
                Email = email,
                NormalizedEmail = email.ToUpper(),
                EmailConfirmed = true,
                PasswordHash = passwordHash,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                PhoneNumber = (string)null,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnd = (DateTimeOffset?)null,
                LockoutEnabled = true,
                AccessFailedCount = 0
            });

        // Controleer en voeg rol toe indien nodig
        var existingRole = await _connection.QueryFirstOrDefaultAsync<string>(
            "SELECT Id FROM AspNetRoles WHERE NormalizedName = @NormalizedName",
            new { NormalizedName = role.ToUpper() });

        string roleId;
        if (existingRole == null)
        {
            // Rol toevoegen
            roleId = Guid.NewGuid().ToString();
            await _connection.ExecuteAsync(@"
                    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
                    VALUES (@Id, @Name, @NormalizedName, @ConcurrencyStamp)",
                new
                {
                    Id = roleId,
                    Name = role,
                    NormalizedName = role.ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                });
        }
        else
        {
            roleId = existingRole;
        }

        // Koppel gebruiker aan rol
        await _connection.ExecuteAsync(@"
                INSERT INTO AspNetUserRoles (UserId, RoleId)
                VALUES (@UserId, @RoleId)",
            new { UserId = userId, RoleId = roleId });

        Console.WriteLine($"Gebruiker '{userName}' succesvol toegevoegd met rol '{role}'");
    }
}
