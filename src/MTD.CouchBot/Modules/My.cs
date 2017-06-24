using Discord.Commands;
using MTD.CouchBot.Domain;
using MTD.CouchBot.Domain.Models.Bot;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MTD.CouchBot.Modules
{
    [Group("my")]
    public class My : ModuleBase
    {
        [Command("birthday"), Summary("Sets users birthday.")]
        public async Task Birthday(string date)
        {
            string file = Constants.ConfigRootDirectory + Constants.UserDirectory + Context.Message.Author.Id + ".json";

            var user = new User();

            if (File.Exists(file))
                user = JsonConvert.DeserializeObject<User>(File.ReadAllText(file));

            if (!string.Equals(date, "clear", StringComparison.CurrentCultureIgnoreCase))
            {
                user.Id = Context.Message.Author.Id;
                try
                {
                    user.Birthday = Convert.ToDateTime(date);
                    File.WriteAllText(file, JsonConvert.SerializeObject(user));
                    await Context.Channel.SendMessageAsync("Your Birthday has been set.");
                }
                catch(FormatException)
                {
                    await Context.Channel.SendMessageAsync("Correct Format: mm/dd/yyyy");
                }
            }
            else
            {
                user.Id = Context.Message.Author.Id;
                user.Birthday = null;
                File.WriteAllText(file, JsonConvert.SerializeObject(user));
                await Context.Channel.SendMessageAsync("Your Birthday has been cleared.");
            }
        }

        [Command("timezoneoffset"), Summary("Sets users time zone offset.")]
        public async Task TimeZoneOffset(float offset)
        {
            string file = Constants.ConfigRootDirectory + Constants.UserDirectory + Context.Message.Author.Id + ".json";

            var user = new User();

            if (File.Exists(file))
            {
                user = JsonConvert.DeserializeObject<User>(File.ReadAllText(file));
            }

            user.TimeZoneOffset = offset;
            File.WriteAllText(file, JsonConvert.SerializeObject(user));
            await Context.Channel.SendMessageAsync("Your Time Zone Offset has been set.");
        }
    }
}
