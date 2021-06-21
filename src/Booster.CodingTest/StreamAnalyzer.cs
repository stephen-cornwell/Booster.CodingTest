﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Booster.CodingTest
{
    /// <summary>
    /// Library to analyse a text stream.
    /// </summary>
    public static class StreamAnalyzer
    {
        #region "properties"

        static private int _countWord = 0;

        static public int CountWord
        {
            get { return _countWord; }
        }

        static private int _countChar = 0;

        static public int CountChar
        {
            get { return _countChar; }
        }

        static private string _sentence = "";

        static public string Sentence
        {
            get { return _sentence; }
        }

        static private List<string> _smallestWords = new();

        static public List<String> SmallestWords
        {
            get { return _smallestWords; }
        }

        static private List<string> _largestWords = new();

        static public List<String> LargestWords
        {
            get { return _largestWords; }
        }

        static private List<string> _frequentWords = new();

        static public List<string> FrequentWords
        {
            get { return _frequentWords; }
        }

        static private List<char> _frequentChars = new();

        static public List<char> FrequentChars
        {
            get { return _frequentChars; }
        }

        private static readonly Dictionary<string, int> AppearingWord = new();
        private static readonly Dictionary<char, int> AppearingChar = new();

        #endregion "properties"

        /// <summary>
        /// Processes the text in Stream format.
        /// Calulculate statistic about the text such as the frequency of words and charaters.
        /// Inform about the smallest and largest word in real time.
        /// </summary>
        /// <param name="text">The text to process in a Stream format.</param>
        /// <param name="delay">if set to <c>true</c> [output].</param>
        /// <returns></returns>
        public static void ProcessText(Stream text, int delay = 0)
        {
            Init();
            string word = "";

            while (true)
            {
                int b = text.ReadByte();

                //end of the stream
                if (b == -1) return;

                char c = Convert.ToChar(b);

                if (Char.IsLetter(c))
                {
                    word += Char.ToLower(c);
                }
                else if (word != "")
                {
                    if (AppearingWord.ContainsKey(word))
                    {
                        AppearingWord[word]++;
                    }
                    else
                    {
                        AppearingWord.Add(word, 1);
                    }

                    UpdateList(ref _smallestWords, word, 5, SmallestWord);
                    UpdateList(ref _largestWords, word, 5, LargestWord);
                    UpdateList(ref _frequentWords, word, 10, FrequentWord);

                    _countWord++;
                    word = "";
                }

                if (AppearingChar.ContainsKey(c))
                {
                    AppearingChar[c]++;
                }
                else
                {
                    AppearingChar.Add(c, 1);
                }

                UpdateList(ref _frequentChars, c, 0, FrequentChar);
                _countChar++;

                _sentence += c;

                try
                {
                    if (_sentence.Length > Console.WindowWidth)
                    {
                        _sentence = _sentence[1..];
                    }
                }
                catch (IOException)
                {
                    if (_sentence.Length > 25)
                    {
                        _sentence = _sentence[1..];
                    }
                }

                //slow down the process of the stream
                if (delay > 0) Thread.Sleep(delay);
            }
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <returns></returns>
        private static void Init()
        {
            _countWord = 0;
            _countChar = 0;
            _sentence = "";
            _smallestWords = new();
            _largestWords = new();
            _frequentWords = new();
            _frequentChars = new();
            AppearingWord.Clear();
            AppearingChar.Clear();
        }

        private static int SmallestWord(string x, string y) => x.Length.CompareTo(y.Length);

        private static int LargestWord(string x, string y) => y.Length.CompareTo(x.Length);

        private static int FrequentWord(string x, string y) => AppearingWord[y].CompareTo(AppearingWord[x]);

        private static int FrequentChar(char x, char y) => AppearingChar[y].CompareTo(AppearingChar[x]);

        /// <summary>
        /// Updates the words list.
        /// </summary>
        /// <param name="list">The list to update.</param>
        /// <param name="newItem">The new item to compare with the list.</param>
        /// <param name="size">The max size of the list. No size limite if size is null.</param>
        /// <param name="compareRule">The comparing rule: smallestWord, largestWord, frequentWord or frequentChar.</param>
        private static void UpdateList<T>(ref List<T> list, T newItem, int size, Comparison<T> compareRule)
        {
            if (!list.Contains(newItem))
            {
                list.Add(newItem);
            }

            list.Sort(compareRule);

            //do not trunc the list if count is null
            if (size > 0 && list.Count > size)
            {
                list.RemoveAt(size);
            }
        }

        public static double GetCharFrequency(char character)
        {
            if (CountChar > 0)
            {
                return Math.Round(AppearingChar[character] / (double)CountChar * 100, 2);
            }
            else
            {
                return 0;
            }
            
        }
    }
}