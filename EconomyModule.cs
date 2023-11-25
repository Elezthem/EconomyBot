using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;

public class EconomyModule : ModuleBase<SocketCommandContext>
{
    private const string EconomyFilePath = "economy.json";

    [Command("work")]
    public async Task Work()
    {
        var user = GetUser();
        var earnings = new Random().Next(10, 101); // Здесь можно установить свои правила для заработка
        user.Balance += earnings;
        SaveUserData(user);
        await ReplyAsync($"Вы отработали и заработали {earnings} монет!");
    }

    [Command("balance")]
    public async Task CheckBalance()
    {
        var user = GetUser();
        await ReplyAsync($"Ваш текущий баланс: {user.Balance} монет.");
    }

    private UserData GetUser()
    {
        var usersData = LoadUserData();
        var userId = Context.User.Id.ToString();

        if (!usersData.ContainsKey(userId))
        {
            usersData[userId] = new UserData
            {
                UserId = userId,
                Balance = 0
            };
            SaveUserData(usersData);
        }

        return usersData[userId];
    }

    private Dictionary<string, UserData> LoadUserData()
    {
        if (File.Exists(EconomyFilePath))
        {
            var json = File.ReadAllText(EconomyFilePath);
            return JsonConvert.DeserializeObject<Dictionary<string, UserData>>(json);
        }

        return new Dictionary<string, UserData>();
    }

    private void SaveUserData(UserData user)
    {
        var usersData = LoadUserData();
        usersData[user.UserId] = user;
        SaveUserData(usersData);
    }

    private void SaveUserData(Dictionary<string, UserData> usersData)
    {
        var json = JsonConvert.SerializeObject(usersData, Formatting.Indented);
        File.WriteAllText(EconomyFilePath, json);
    }
}

public class UserData
{
    public string UserId { get; set; }
    public decimal Balance { get; set; }
}
