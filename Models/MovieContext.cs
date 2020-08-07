using MySql.Data.MySqlClient;
using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MovieApi.Models
{
  public class MovieContext
  {
    public string ConnectionString { get; set; }
    private readonly ILogger _logger;
    public MovieContext(string connectionString,ILogger<MovieContext> logger)
    {
      this.ConnectionString = connectionString;
      _logger = logger;
    }
    private MySqlConnection GetConnection()
    {
      return new MySqlConnection(ConnectionString);
    }

    public async Task<List<MovieItem>> GetAllItems(CancellationToken cancellationToken)
    {
      List<MovieItem> list = new List<MovieItem>();
      using (MySqlConnection conn = GetConnection()) 
      {
         using( MySqlCommand cmd =  conn.CreateCommand())
         {
           try
            {
              cmd.CommandText = "SELECT id,name,year,poster,posterSize FROM item"; 
              cmd.CommandType = System.Data.CommandType.Text;
              cmd.Connection = conn;
              conn.Open();
              using (MySqlDataReader reader =await cmd.ExecuteReaderAsync(cancellationToken) as MySqlDataReader)
              {
                while (reader.Read())
                {
                  MovieItem item = new MovieItem();
                  item.id = GetDefaultInt32(reader,"id");
                  item.name = GetNullString(reader,"name");
                  item.year = GetDefaultInt32(reader,"year");
                  item.posterSize = GetDefaultInt32(reader,"posterSize");
                  item.poster = new byte[item.posterSize];
                  if(item.posterSize > 0)
                  {
                    reader.GetBytes(3,0,item.poster,0,item.posterSize);
                  }              
                  list.Add(item);
                }
              }
            }
            catch(MySqlException e)
            {
              _logger.LogInformation("Get all items query is cancelled. " + e.Message); 
            }
            catch(TaskCanceledException e)
            {
              _logger.LogInformation("Get all items task is cancelled. " + e.Message); 
            }
            
         }
        conn.Close();
      }
      _logger.LogInformation("Get all items is successfull.");
      return list;
    }

    public async Task<MovieItem> GetAnItem(CancellationToken cancellationToken, int id)
    {
        using (MySqlConnection conn = GetConnection())
        {
          try
          {
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(null, conn);
            cmd.CommandText = "SELECT id, name, year, poster,posterSize  FROM item WHERE id= @id"; 
            cmd.CommandType = System.Data.CommandType.Text;
            MySqlParameter idParameter = new MySqlParameter("@id", MySqlDbType.Int32, 0);

            idParameter.Value = id;
            cmd.Parameters.Add(idParameter);
            cmd.Prepare();

            using (MySqlDataReader reader = await cmd.ExecuteReaderAsync(cancellationToken) as MySqlDataReader)
            {
              while (reader.Read())
              {
                MovieItem item = new MovieItem();
                item.id = GetDefaultInt32(reader,"id");
                item.name = GetNullString(reader,"name");
                item.year = GetDefaultInt32(reader,"year");
                item.posterSize = GetDefaultInt32(reader,"posterSize");
                item.poster = new byte[item.posterSize];
                if(item.posterSize > 0)
                {
                  reader.GetBytes(3,0,item.poster,0,item.posterSize);
                }
                else{
                  item.poster = null;
                }
                conn.Close();
                return item;
              }
            }
          }
          catch(MySqlException e)
            {
              _logger.LogInformation("Get an item query is cancelled. " + e.Message); 
            }
            catch(TaskCanceledException e)
            {
              _logger.LogInformation("Get an item task is cancelled." + e.Message); 
            }
        }
        _logger.LogInformation("Get an item is successfull.");
        return null;
      }

    public MovieItem PostItem(MovieItem item) 
    {
      using (MySqlConnection conn = GetConnection())
      {
        conn.Open();
        MySqlCommand cmd = new MySqlCommand(null, conn);
        cmd.CommandText = "INSERT into item (name,year,poster,posterSize)" + "VALUES (@name, @year,@poster,@posterSize)"; 

        MySqlParameter nameParameter = new MySqlParameter("@name", MySqlDbType.Text, 20);
        MySqlParameter yearParameter = new MySqlParameter("@year", MySqlDbType.Int32, 0);
        MySqlParameter posterParameter = new MySqlParameter("@poster", MySqlDbType.MediumBlob);
        MySqlParameter posterSizeParameter = new MySqlParameter("@posterSize", MySqlDbType.Int32);

        nameParameter.Value = item.name;
        yearParameter.Value = item.year;
        posterParameter.Value = item.poster;
        if(item.poster == null)
        {
          posterSizeParameter.Value = 0;
        }
        else
        {
          posterSizeParameter.Value = item.poster.Length;
        }

        cmd.Parameters.Add(nameParameter);
        cmd.Parameters.Add(yearParameter);
        cmd.Parameters.Add(posterParameter);
        cmd.Parameters.Add(posterSizeParameter);

        cmd.Prepare();
        cmd.ExecuteNonQuery();
        conn.Close();
        _logger.LogInformation("Post item is successful.");
        return item;
      }
    }
    public async Task<MovieItem> PutItem(CancellationToken cancellationToken, MovieItem movieItem)
    {
      using (MySqlConnection conn = GetConnection()) 
      {
          conn.Open();
          MySqlCommand cmd = new MySqlCommand(null, conn); 
          cmd.CommandText = "UPDATE item SET name=@name, year=@year, poster=@poster, posterSize = @posterSize WHERE id=@id";

          MySqlParameter idParameter = new MySqlParameter("@id", MySqlDbType.Int32, 0);
          MySqlParameter nameParameter = new MySqlParameter("@name", MySqlDbType.Text, 20);
          MySqlParameter yearParameter = new MySqlParameter("@year", MySqlDbType.Int32, 0);
          MySqlParameter posterParameter = new MySqlParameter("@poster", MySqlDbType.MediumBlob);
          MySqlParameter posterSizeParameter = new MySqlParameter("@posterSize", MySqlDbType.Int32); 

          idParameter.Value = movieItem.id;
          if(movieItem.name != null)
          {
            nameParameter.Value = movieItem.name;
          }
          yearParameter.Value = movieItem.year;
          posterParameter.Value = movieItem.poster;
          if(movieItem.poster == null)
          {
            MovieItem item = await GetAnItem(cancellationToken ,movieItem.id);
            movieItem.poster = item.poster;
            posterSizeParameter.Value = item.posterSize; 
          }
          else
          {
            posterSizeParameter.Value = movieItem.poster.Length;
          }

          cmd.Parameters.Add(idParameter);
          cmd.Parameters.Add(nameParameter);
          cmd.Parameters.Add(yearParameter);
          cmd.Parameters.Add(posterParameter);
          cmd.Parameters.Add(posterSizeParameter);
          

          cmd.Prepare();
          cmd.ExecuteNonQuery();
          conn.Close();
          _logger.LogInformation("Put item is successful.");
          return movieItem;
      }
    }
    public void DeleteItem( int id)
    {
      using (MySqlConnection conn = GetConnection())
      {
        conn.Open();
        MySqlCommand cmd = new MySqlCommand(null, conn);

        cmd.CommandText= "DELETE FROM item WHERE id =@id";
        MySqlParameter idParameter = new MySqlParameter("@id", MySqlDbType.Int32, 0);

        idParameter.Value = id;
        cmd.Parameters.Add(idParameter);

        cmd.Prepare();       
        cmd.ExecuteNonQuery();
        _logger.LogInformation("Delete item is successful");
        conn.Close();
      }
    }

    private string GetNullString(MySqlDataReader reader, string str)
    {
      if(!reader.IsDBNull(reader.GetOrdinal(str)))
      {
        return reader.GetString(str);
      }
      return str;
    }

    private int GetDefaultInt32(MySqlDataReader reader, string integer)
    {
      if(!reader.IsDBNull(reader.GetOrdinal(integer)))
      {
        return reader.GetInt32(integer);
      }
      return -1;
    }
    
  }
}
