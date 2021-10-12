using CustomEmoji.Source.CustomEmojiInterface;
using CustomEmoji.Source.Provider;
using System;
using System.Drawing;
using System.IO;

namespace CustomEmoji
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            TestCustomEmojiProvider();
        }

        private static void TestCustomEmojiProvider()
        {
            Console.WriteLine("Start TestCustomEmojiProvider");
            CustomEmojiProvider provider = new CustomEmojiProvider();

            var existingCustomEmojiRecords = provider.GetPublicCustomEmojiEntries();
            if (existingCustomEmojiRecords != null)
            {
                foreach (var record in existingCustomEmojiRecords)
                {
                    Console.WriteLine($"Id: {record.Id}, Name: {record.Name}, DisplayName: {record.DisplayName}, Description: {record.Description}, AuthorUserObjectId: {record.AuthorUserObjectId}, IngestedTime: {record.IngestedTime}, State: {record.State}.");
                }
            }
        }
    }
}
