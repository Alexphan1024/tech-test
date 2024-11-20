using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace BackendAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class AvatarController : ControllerBase
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        private string connectionString = "Data Source=Data/data.db";
        private bool IsVowel(string letter) => letter.ToLower().Any(c => "aeiou".Contains(c));
        private bool IsSymbol(string letter) => letter.Any(c => !char.IsLetterOrDigit(c));
        private string GetImageFromDB(int id)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            string query = "SELECT url FROM images WHERE id = @id";
            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);

            using var reader = command.ExecuteReader();
            return reader.Read() ? reader.GetString(0) : string.Empty;
        }

    [HttpGet("avatar")]
    public IActionResult GetAvatar([FromQuery] string userIdentifier)
    {
        if (string.IsNullOrEmpty(userIdentifier)) 
            return BadRequest("Invalid");

        string imageUrl = string.Empty;

        char lastChar = userIdentifier[^1];

        if (char.IsDigit(lastChar))
        {
            int lastDigit = int.Parse(lastChar.ToString());

            if (lastDigit >= 6 && lastDigit <= 9)
            {
                imageUrl = $"https://my-json-server.typicode.com/ck-pacificdev/tech-test/images/{lastDigit}";
            }
            else if (lastDigit >= 1 && lastDigit <= 5)
            {
                imageUrl = GetImageFromDB(lastDigit);
            }
        }

        if (string.IsNullOrEmpty(imageUrl))
        {
            string seedValue = "default";

            if (IsVowel(userIdentifier))
            {
                seedValue = "vowel";
            }
            else if (IsSymbol(userIdentifier))
            {
                var random = new Random();
                seedValue = random.Next(1, 6).ToString();
            }

            imageUrl = $"https://api.dicebear.com/8.x/pixel-art/png?seed={seedValue}&size=150";
        }

        return Ok(new { url = imageUrl });
    }
    }
}
