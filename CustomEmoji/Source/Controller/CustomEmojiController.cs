namespace CustomEmoji.Source.Controller
{
    using System.Collections.Generic;
    using CustomEmoji.Source.CustomEmojiInterface;
    using CustomEmoji.Source.Provider;
    using Microsoft.AspNetCore.Mvc;

    public class CustomEmojiController
    {
        private ICustomEmojiProvider provider = new CustomEmojiProvider();

        [HttpGet]
        [Route("customEmoji/publicEmojis")]
        public IList<CustomEmojiEntry> GetPublicCustomEmojis()
        {
            return this.provider.GetPublicCustomEmojiEntries();
        }

        [HttpGet]
        [Route("{userId}/customEmoji/userEmojis")]
        public IList<CustomEmojiEntry> GetCustomEmojisByUserId([FromRoute] string userId)
        {
            return this.provider.GetCustomEmojiEntriesByUserId(userId);
        }

        [HttpPost]
        [Route("customEmoji/emoji")]
        public void CreateCustomEmoji([FromBody] CustomEmojiEntry emoji)
        {
            this.provider.TryAddCustomEmojiEntry(emoji);
        }

        [HttpPut]
        [Route("customEmoji/emoji")]
        public void UpdateCustomEmoji([FromBody] CustomEmojiEntry emoji)
        {
            this.provider.TryAddOrUpdateCustomEmojiEntry(emoji);
        }

        [HttpDelete]
        [Route("{userId}/customEmoji/{emojiId}")]
        public void DeleteAppEntitlement([FromRoute] string userId, [FromRoute] string emojiId)
        {
            this.provider.TryInactiveExistingCustomEmojiEntry(emojiId, userId);
        }
    }
}
