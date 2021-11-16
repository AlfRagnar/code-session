﻿using Microsoft.AspNetCore.Mvc;

namespace net6_react.Controllers;

public record User(int Id, string Name, string Email, IEnumerable<string> Roles, int? BossId);

[ApiController]
[Route("[controller]")]
public class EmployeesController : ControllerBase
{
    private static readonly User[] Users = new[]
    {
        new User(1, "Ola Olsen", "ola@x.no", new [] { "Daglig leder" }, null),
        new User(101, "Pelle Persen", "pelle@x.no", new [] { "Ukentlig leder" }, 1),
        new User(201, "Arne Arntsen", "arne@x.no", new [] { "Dreng" }, 101),
        new User(202, "Ulle Urvik", "beanie_baby_collector_2291@hotmail.com", new [] { "Byssegutt" }, 101),
    };

    [HttpGet]
    public async Task<IEnumerable<User>> GetAsync(int delay = 1000)
    {
        await Task.Delay(delay);
        return Users;
    }

    [HttpGet, Route("modified")]
    public IEnumerable<User> GetModified()
    {
        // Returner brukerne, men med navn uppercased og negativ userId (101 => -101)
        // Organisasjons-strukturen må bevares (Id / BossId)
        IList<User> modifyUsers = new List<User>();

        for (int i = 0; i < Users.Length; i++)
        {
            var negativeID = Users[i].Id * -1;
            modifyUsers.Add(new User(negativeID, Users[i].Name.ToUpper(), Users[i].Email, Users[i].Roles, Users[i].BossId));
        }

        return modifyUsers;
    }

    [HttpGet, Route("boss")]
    public User? GetTheBoss()
    {
        User? boss = Users.FirstOrDefault(x => x.BossId == null);
        return boss;
    }

    [HttpGet, Route("fakes/{count:int}")]
    public IEnumerable<User> GetFakes(int count)
    {
        // Generer {count} fiktive ansatte

        throw new NotImplementedException();
    }

    [HttpGet, Route("names")]
    public IEnumerable<string> GetEmployeeNames()
    {
        return Users.Select(x => x.Name);
    }

    [HttpGet, Route("search/{query}")]
    public IEnumerable<User> FindUsers(string query)
    {
        // Søk på navn, e-post, roller
        try
        {
            var enumerator = Users.GetEnumerator();
            IList<User> result = new List<User>();
            while (enumerator.MoveNext())
            {
                User user = (User)enumerator.Current;
                if (user.Name.ToUpper().Contains(query.ToUpper()))
                {
                    result.Add(user);
                }
                else if (user.Email.ToUpper().Contains(query.ToUpper()))
                {
                    result.Add(user);
                }
                else if (user.Roles.Any())
                {
                    var userRoles = user.Roles.ToList();
                    foreach (var role in userRoles)
                    {
                        if (role.ToUpper().Contains(query.ToUpper()))
                        {
                            result.Add(user);
                        }
                    }
                }
            }
            return result;
        }
        catch (Exception)
        {
            return null!;
        }
    }
}