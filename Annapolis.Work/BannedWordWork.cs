using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Entity;
using Annapolis.Abstract.Work;

namespace Annapolis.Work
{
    public class BannedWordWork : AnnapolisBaseCacheCrudWork<ContentBannedWord>, IBannedWordWork
    {
        internal TrieFilter WordFilter
        {
            get 
            {
                if (!CacheManager.Contains("BannedWordService_WordFilter"))
                {
                    var filter = new TrieFilter();
                    var list = AllCacheItems.Where(x => x.IsRequiredToCheck).Select(x => x.Word).Distinct();
                    foreach(var word in list)
                    {
                        filter.AddKey(word.ToLower());
                    }
                    CacheManager.AddOrUpdate("BannedWordService_WordFilter", filter);
                }

                return CacheManager.GetData<TrieFilter>("BannedWordService_WordFilter");
            }
        }

        public string ReplaceBannedWord(string text, char replaceHolder)
        {
           if(string.IsNullOrEmpty(text)) return text;
           return WordFilter.Replace(text, replaceHolder);
        }

    }

    internal class TrieNode
    {
        public bool End;
        public Dictionary<Char, TrieNode> Values;
        public TrieNode()
        {
            Values = new Dictionary<Char, TrieNode>();
        }
    }

    internal class TrieFilter : TrieNode
    {
        public void AddKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }
            TrieNode node = this;
            for (int i = 0; i < key.Length; i++)
            {
                char c = key[i];
                TrieNode subnode;
                if (!node.Values.TryGetValue(c, out subnode))
                {
                    subnode = new TrieNode();
                    node.Values.Add(c, subnode);
                }
                node = subnode;
            }
            node.End = true;
        }

        public bool HasBadWord(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                TrieNode node;
                if (Values.TryGetValue(Char.ToLower(text[i]), out node))
                {
                    for (int j = i + 1; j < text.Length; j++)
                    {
                        if (node.Values.TryGetValue(Char.ToLower(text[j]), out node))
                        {
                            if (node.End)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            return false;
        }

        public string FindOne(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                TrieNode node;
                if (Values.TryGetValue(Char.ToLower(text[i]), out node))
                {
                    for (int j = i + 1; j < text.Length; j++)
                    {
                        if (node.Values.TryGetValue(Char.ToLower(text[j]), out node))
                        {

                            if (node.End)
                            {
                                return text.Substring(i, j + 1 - i);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            return string.Empty;
        }

        public IEnumerable<string> FindAll(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                TrieNode node;
                if (Values.TryGetValue(Char.ToLower(text[i]), out node))
                {
                    for (int j = i + 1; j < text.Length; j++)
                    {
                        if (node.Values.TryGetValue(Char.ToLower(text[j]), out node))
                        {
                            if (node.End)
                            {
                                yield return text.Substring(i, (j + 1 - i));
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }


        public string Replace(string text, char c)
        {
            char[] chars = null;
            for (int i = 0; i < text.Length; i++)
            {
                TrieNode subnode;
                if (Values.TryGetValue(Char.ToLower(text[i]), out subnode))
                {
                    for (int j = i + 1; j < text.Length; j++)
                    {
                        if (subnode.Values.TryGetValue(Char.ToLower(text[j]), out subnode))
                        {
                            if (subnode.End)
                            {
                                if (chars == null) chars = text.ToArray();
                                for (int t = i; t <= j; t++)
                                {
                                    chars[t] = c;
                                }
                                i = j;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            return chars == null ? text : new string(chars);
        }
    }
}
