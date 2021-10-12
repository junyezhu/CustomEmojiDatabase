namespace CustomEmoji
{
    using CustomEmoji.Source.CustomEmojiInterface;
    using CustomEmoji.Source.Provider;
    using System;
    using System.Drawing;
    using System.IO;

    class Program
    {
        static void Main(string[] args)
        {
            TestCustomEmojiProvider();
        }

        private static void TestCustomEmojiProvider()
        {
            Console.WriteLine("Start TestCustomEmojiProvider");
            CustomEmojiProvider provider = new CustomEmojiProvider();
            Console.WriteLine();

            Console.WriteLine("Test method TryAddCustomEmojiEntry with duplicate id:");
            if(provider.TryAddCustomEmojiEntry(GetMockCustomEmojiEntry("655a59cf-25be-4948-8057-a567f816bdcc")))
            {
                throw new Exception("TryAddCustomEmojiEntry method is able to add CustomEmojiEntry with existing id");
            }
            else
            {
                Console.WriteLine("TryAddCustomEmojiEntry method return false for entry with duplicate id.");
            }
            Console.WriteLine();

            Console.WriteLine("GetPublicCustomEmojiEntries:");
            var existingCustomEmojiRecords = provider.GetPublicCustomEmojiEntries();
            if (existingCustomEmojiRecords != null)
            {
                foreach (var record in existingCustomEmojiRecords)
                {
                    Console.WriteLine($"Id: {record.Id}, Name: {record.Name}, DisplayName: {record.DisplayName}, Description: {record.Description}, AuthorUserObjectId: {record.AuthorUserObjectId}, IngestedTime: {record.IngestedTime}, State: {record.State}.");
                }
            }
            Console.WriteLine();

            Console.WriteLine($"GetCustomEmojiEntriesByUserId for userId 43f286b0-218c-407e-a073-8ebf99fb6361:");
            existingCustomEmojiRecords = provider.GetCustomEmojiEntriesByUserId("43f286b0-218c-407e-a073-8ebf99fb6361");
            if (existingCustomEmojiRecords != null)
            {
                foreach (var record in existingCustomEmojiRecords)
                {
                    Console.WriteLine($"Id: {record.Id}, Name: {record.Name}, DisplayName: {record.DisplayName}, Description: {record.Description}, AuthorUserObjectId: {record.AuthorUserObjectId}, IngestedTime: {record.IngestedTime}, State: {record.State}.");
                }
            }
            Console.WriteLine();

            Console.WriteLine($"GetCustomEmojiEntriesByUserId for userId 57bd0ae3-ef1a-461c-896c-f3cdf896013f:");
            existingCustomEmojiRecords = provider.GetCustomEmojiEntriesByUserId("57bd0ae3-ef1a-461c-896c-f3cdf896013f");
            if (existingCustomEmojiRecords != null)
            {
                foreach (var record in existingCustomEmojiRecords)
                {
                    Console.WriteLine($"Id: {record.Id}, Name: {record.Name}, DisplayName: {record.DisplayName}, Description: {record.Description}, AuthorUserObjectId: {record.AuthorUserObjectId}, IngestedTime: {record.IngestedTime}, State: {record.State}.");
                }
            }
        }

        private static CustomEmojiEntry GetMockCustomEmojiEntry( string id)
        {
            Image gifImg = Image.FromFile(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\EmojiGifs\\cool.gif");
            byte[] gifArray = null;
            using (var ms = new MemoryStream())
            {
                gifImg.Save(ms, gifImg.RawFormat);
                gifArray = ms.ToArray();
            }

            return new CustomEmojiEntry
            {
                Id = id,
                Name = "Cool",
                DisplayName = "Cool",
                Description = "Wear sunglasses gif",
                AuthorUserObjectId = "43f286b0-218c-407e-a073-8ebf99fb6361",
                IngestedTime = DateTime.Now,
                State = CustomEmojiEntryState.Public,
                Content = gifArray
            };
        }
    }
}
