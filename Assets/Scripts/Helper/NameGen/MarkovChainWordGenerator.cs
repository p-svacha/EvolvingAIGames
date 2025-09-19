using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MarkovChainWordGenerator
{
    private const char WordStartChar = '˨';
    private const char WordEndChar = '˩';
    private const string MultiLineEndInput = "---ENDINPUT---";

    public int MinNGramLength = 2;
    public int MaxNGramLength = 12;

    public int MinLanguageWeight = 0;
    public int MaxLanguageWeight = 100;

    /// <summary>
    /// The initial key represents the word type (i.e. "Planet" or "Country")
    /// The key of the first dictionary is the nGram length.
    /// The final dictionary: key is the ngram string and the int the amount of occurences in the input data set
    /// </summary>
    private Dictionary<string, Dictionary<int, Dictionary<string, int>>> NGrams;

    /// <summary>
    /// Key is the word type  (i.e. "Planet" or "Country")
    /// Value is a list with all input words
    /// </summary>
    public Dictionary<string, List<string>> InputWords;

    public MarkovChainWordGenerator()
    {
        NGrams = new Dictionary<string, Dictionary<int, Dictionary<string, int>>>();
        InputWords = new Dictionary<string, List<string>>();

        foreach (string category in NameGeneratorInputDataReader.WordCategories.Keys)
        {
            List<string> inputWords = NameGeneratorInputDataReader.GetInputWords(category);
            InputWords.Add(category, inputWords);

            NGrams.Add(category, new Dictionary<int, Dictionary<string, int>>());
            foreach(string word in inputWords)
            {
                for (int i = MinNGramLength; i <= MaxNGramLength; i++)
                {
                    CreateNGramsFor(category, word, i);
                }
            }
        }
    }


    public string GenerateWord(string wordType, int nGramLength, string start = "", int maxLength = -1)
    {
        //Debug.Log("##################### NEW WORD #######################");
        string word = WordStartChar + start + "";

        while (!word.EndsWith(WordEndChar + ""))
        {
            //Debug.Log(word);
            if (word.Length < (nGramLength - 1))
            {
                int startIndex = 0;
                int length = word.Length;
                string nGram = PickRandomNGramStartingWith(word, wordType, word.Substring(startIndex, length), nGramLength);
                word += nGram[word.Length];
            }
            else
            {
                int startIndex = word.Length - (nGramLength - 1);
                int length = nGramLength - 1;
                string nGram = PickRandomNGramStartingWith(word, wordType, word.Substring(startIndex, length), nGramLength);
                word += nGram[nGramLength - 1];
            }

            // Reset if over max length
            if (maxLength != -1 && word.Length - 2 > maxLength)
            {
                word = WordStartChar + start + "";
            }
        }

        return word.Substring(1, word.Length - 2); // Remove word start and end char
    }

    private string PickRandomNGramStartingWith(string wordSoFar, string wordType, string nGramStart, int nGramLength)
    {
        Dictionary<string, int> candidateNGrams = NGrams[wordType][nGramLength].Where(x => StartsWith(x.Key, nGramStart)).ToDictionary(x => x.Key, x => x.Value);
        int totalProbability = candidateNGrams.Sum(x => x.Value);

        // Create array where each ngram has as many occurences as it has in the original list
        string[] weightedArray = new string[totalProbability];
        int id = 0;
        foreach(KeyValuePair<string, int> kvp in candidateNGrams)
            for (int i = 0; i < kvp.Value; i++) weightedArray[id++] = kvp.Key;

        // Chose one random entry in the weighted array
        if (weightedArray.Length == 0) throw new Exception("No nGram found that starts with " + nGramStart);
        string chosenNgram = weightedArray[UnityEngine.Random.Range(0, weightedArray.Length)];
        byte[] bytes = Encoding.UTF8.GetBytes(chosenNgram);
        string encodedNGram = Encoding.Default.GetString(bytes);

        //string options = "";
        //foreach (KeyValuePair<string, int> kvp in candidateNGrams) options += "\n" + kvp.Key + " (" + kvp.Value + ")";
        //Debug.Log("Chosen nGram: " + chosenNgram + " out of " + candidateNGrams.Count + " options. Listing options:" + options);
        
        return encodedNGram;
    }

    private void CreateNGramsFor(string wordType, string word, int nGramLength)
    {
        if (word.Length < (nGramLength - 1)) return; // Skip word if shorter than nGramLength - 1
        string nGram = WordStartChar + word.Substring(0, nGramLength - 1);
        AddNGram(wordType, nGram, nGramLength);
        for(int i = 0; i < word.Length; i++)
        {
            if (i < word.Length - (nGramLength - 1)) nGram = word.Substring(i, nGramLength);
            else nGram = word.Substring(i, word.Length - i) + WordEndChar;
            AddNGram(wordType, nGram, nGramLength);
        }
    }

    private void AddNGram(string wordType, string ngram, int nGramLength)
    {
        if (!NGrams[wordType].ContainsKey(nGramLength)) NGrams[wordType].Add(nGramLength, new Dictionary<string, int>());
        if (NGrams[wordType][nGramLength].ContainsKey(ngram)) NGrams[wordType][nGramLength][ngram]++;
        else NGrams[wordType][nGramLength].Add(ngram, 1);
    }

    private bool StartsWith(string nGram, string start)
    {
        for(int i = 0; i < start.Length; i++)
        {
            if (nGram[i] != start[i]) return false;
        }
        return true;
    }

    public string GetRandomNgram(string category, int length, int minOccurences)
    {
        Dictionary<string, int> ngrams = NGrams[category][length];
        List<string> candidateNgrams = ngrams.Where(x => !x.Key.Contains(WordStartChar) && !x.Key.Contains(WordEndChar) && x.Value >= minOccurences).Select(x => x.Key).ToList();
        return candidateNgrams[UnityEngine.Random.Range(0, candidateNgrams.Count)];
    }
}
