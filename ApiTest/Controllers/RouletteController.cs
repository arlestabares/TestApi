using BusinessEntities.DataBaseEntities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApiTest.Controllers
{
    [ApiController]
    [Route("roulettes")]
    public class RouletteController : ControllerBase
    {
        private readonly string DataBaseHost = Environment.GetEnvironmentVariable("DATA_BASE_HOST");
        private readonly string DataBaseUser = Environment.GetEnvironmentVariable("DATA_BASE_USER");
        private readonly string DataBasePassword = Environment.GetEnvironmentVariable("DATA_BASE_PASSWORD");
        private readonly string DataBaseName = Environment.GetEnvironmentVariable("DATA_BASE_NAME");
        private readonly NpgsqlConnection connection;
        private int idRoulette;

        public RouletteController()
        {
            var connectionString = "Host" + DataBaseHost + ";Database=" + DataBaseName + ";Username=" + DataBaseUser + ";Password=" + DataBasePassword;
            connection = new NpgsqlConnection(connectionString);

        }

        [HttpPost("/create-roulette")]
        public async Task<int> CreateRoulette([FromBody] Roulette roulette)
        {
            await connection.OpenAsync();
            int idRoulette = 0;
            StringBuilder query = new StringBuilder();
            query.Append("INSERT INTO roulettes  (roulette_name,is_open) VALUES (@Name, @Is_open) ");
            using (NpgsqlCommand command = new NpgsqlCommand(query.ToString(), connection))
            {
                command.Parameters.AddWithValue("Name", roulette.roulette_name);
                command.Parameters.AddWithValue("Is_open", roulette.roulette_id);
                idRoulette = await command.ExecuteNonQueryAsync();
                await connection.CloseAsync();
            }
            return idRoulette;

        }
        [HttpPut("/activate-roulette")]
        public async Task<string> ActivateRoulette([FromBody] Roulette roulettePut)
        {
            Roulette roulette = await GetRouletteById(roulettePut.roulette_id);
            if (roulette != null)
            {
                if (roulette.is_open)
                    return "Successfully update";

                await connection.OpenAsync();
                StringBuilder query = new StringBuilder();
                query.Append("UPDATE roulettes SET is_open = true WHERE roulette_id = @Roulette_id ");
                using (NpgsqlCommand command = new NpgsqlCommand(query.ToString(), connection))
                {
                    command.Parameters.AddWithValue("Roulette_id", roulette.roulette_id);
                    await command.ExecuteNonQueryAsync();
                    await connection.CloseAsync();

                    return "Successfully updated";
                }
            }
            return "There was an error updating the roulette";
        }

        [HttpPut("/disable-roulette")]
        public string DisableRoulette()
        {
            Roulette roulette = new Roulette();
            roulette.roulette_id = 1;
            roulette.roulette_name = "Ruleta 1";
            roulette.is_open = false;

            string rouletteJson = JsonConvert.SerializeObject(roulette);

            return rouletteJson;
        }

        [HttpGet]
        public async Task<List<Roulette>> GetAllRoulettes()
        {
            List<Roulette> listRoulettes = new List<Roulette>();
            await connection.OpenAsync();
            StringBuilder query = new StringBuilder();
            query.Append("SELECT * FROM roulettes");
            using (NpgsqlCommand command = new NpgsqlCommand(query.ToString(), connection))
            {
                using (NpgsqlDataReader dataReader = await command.ExecuteReaderAsync())
                {
                    while (dataReader.Read())
                    {
                        Roulette roulette = new Roulette();
                        roulette.roulette_id = (int)Convert.ToInt64(dataReader[0]);
                        roulette.roulette_name = dataReader[1].ToString();
                        roulette.is_open = Convert.ToBoolean(dataReader[2]);
                        listRoulettes.Add(roulette);
                    }

                }
            }
            await connection.CloseAsync();
            return listRoulettes;
        }

        private async Task<Roulette> GetRouletteById(int rouletteId)
        {
            Roulette roulette = new Roulette();
            await connection.OpenAsync();
            StringBuilder query = new StringBuilder();
            query.Append("SELECT * FROM roulettes WHERE roulette_id =@Roulette_id");

            using (NpgsqlCommand command = new NpgsqlCommand(query.ToString(), connection))
            {
                command.Parameters.AddWithValue("Roulette_id", rouletteId);
                using (NpgsqlDataReader dataReader = await command.ExecuteReaderAsync())
                {
                    while (dataReader.Read())
                    {
                        roulette = new Roulette();
                        roulette.roulette_id = (int)Convert.ToInt64(dataReader[0]);
                        roulette.roulette_name = dataReader[1].ToString();
                        roulette.is_open = Convert.ToBoolean(dataReader[2]);
                    }
                }

            }

            await connection.CloseAsync();
            return roulette;
        }
    }
}
