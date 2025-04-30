using System.Collections.Generic;

namespace TPL.PVZR.Gameplay.Class.Tags
{
    public class TagGroup
    {
        #region 公有方法

        public void Add(Tag tag)
        {
            _tags.Add(tag);
        }

        public void Remove(Tag tag)
        {
            _tags.Remove(tag);
        }

        public bool Contains(Tag tag)
        {
            return _tags.Contains(tag);
        }
        #endregion

        #region 构造函数

        public TagGroup()
        {
            _tags = new List<Tag>();
        }

        public TagGroup(Tag[] tags)
        {
            _tags = new List<Tag>(tags);
        }

        #endregion

        #region 私有

        private readonly List<Tag> _tags;

        #endregion
    }
}