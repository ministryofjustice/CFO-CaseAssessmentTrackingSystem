namespace Cfo.Cats.Infrastructure.Services.Identity;

public class PasswordService(IIdentitySettings identitySettings) : IPasswordService
{
    readonly Dictionary<char, string> phonetics = new() {
        { '0', "Zero" },
        { '1', "One" },
        { '2', "Two" },
        { '3', "Three" },
        { '4', "Four" },
        { '5', "Five" },
        { '6', "Six" },
        { '7', "Seven" },
        { '8', "Eight" },
        { '9', "Nine" },
        { 'a', "Alpha" }, 
        { 'b', "Bravo" }, 
        { 'c', "Charlie" }, 
        { 'd', "Delta" }, 
        { 'e', "Echo" },
        { 'f', "Foxtrot" }, 
        { 'g', "Golf" }, 
        { 'h', "Hotel" }, 
        { 'i', "India" }, 
        { 'j', "Juliet" },
        { 'k', "Kilo" }, 
        { 'l', "Lima" }, 
        { 'm', "Mike" }, 
        { 'n', "November" }, 
        { 'o', "Oscar" },
        { 'p', "Papa" }, 
        { 'q', "Quebec" }, 
        { 'r', "Romeo" }, 
        { 's', "Sierra" }, 
        { 't', "Tango" },
        { 'u', "Uniform" }, 
        { 'v', "Victor" }, 
        { 'w', "Whiskey" }, 
        { 'x', "X-Ray" }, 
        { 'y', "Yankee" },
        { 'z', "Zulu" }, 
        { '@', "At-Sign" }, 
        { '!', "Exclamation-Mark" }, 
        { '?', "Question-Mark" }, 
        { '*', "Asterisk" }, 
        { '.', "Full-Stop" } };
    
    IIdentitySettings IdentitySettings { get; } = identitySettings;

    readonly string[] randomChars = [
        "ABCDEFGHJKLMNOPQRSTUVWXYZ",   // uppercase 
        "abcdefghijkmnopqrstuvwxyz",   // lowercase
        "0123456789",                  // digits
        "@!?*."                        // non-alphanumeric
    ];

    public string GeneratePassword(IIdentitySettings? identitySettings = null)
    {
        identitySettings ??= IdentitySettings;

        Random rand = new (Environment.TickCount);
        List<char> chars = [];

        if (identitySettings.RequireUpperCase)
            chars.Insert(rand.Next(0, chars.Count),
                randomChars[0][rand.Next(0, randomChars[0].Length)]);

        if (identitySettings.RequireLowerCase)
            chars.Insert(rand.Next(0, chars.Count),
                randomChars[1][rand.Next(0, randomChars[1].Length)]);

        if (identitySettings.RequireDigit)
            chars.Insert(rand.Next(0, chars.Count),
                randomChars[2][rand.Next(0, randomChars[2].Length)]);

        if (identitySettings.RequireNonAlphanumeric)
            chars.Insert(rand.Next(0, chars.Count),
                randomChars[3][rand.Next(0, randomChars[3].Length)]);

        for (int i = chars.Count; i < identitySettings.RequiredLength; i++)
        {
            string rcs = randomChars[rand.Next(0, randomChars.Length)];
            chars.Insert(rand.Next(0, chars.Count),
                rcs[rand.Next(0, rcs.Length)]);
        }

        return new string(chars.ToArray());
    }

    public List<KeyValuePair<char, string>> GetPronunciation(string input)
    {
        List<KeyValuePair<char, string>> natoWords = [];

        foreach (char c in input)
        {
            char key = char.ToLower(c);

            if (phonetics.ContainsKey(key))
            {
                natoWords.Add(new KeyValuePair<char, string>(c, phonetics[key]));
            }
            else
            {
                natoWords.Add(new KeyValuePair<char, string>(c, c.ToString())); // Add the character directly if it's not in the dictionary
            }
        }

        return natoWords;
    }

}

public interface IPasswordService
{
    public string GeneratePassword(IIdentitySettings? identitySettings = null);
    public List<KeyValuePair<char, string>> GetPronunciation(string input);
}