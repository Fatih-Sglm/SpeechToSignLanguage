using net.zemberek.erisim;
using net.zemberek.tr.yapi;
using SpeechToSignLanguage.Client.Functions.Video;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpeechToSignLanguage.Client.Functions.Word
{
    public class WordFunction
    {
        readonly List<string> wordsList = new List<string>();
        readonly List<string> mylist = new List<string>(new string[] { "nasılsın" });
        readonly VideoFunciton videoFunciton = new VideoFunciton();
        readonly NumberFunction nf = new NumberFunction();

        public string FindRootOfTheWord(string word)
        {
            var zemberek = new Zemberek(new TurkiyeTurkcesi());
            if (IsWordInList(mylist, word))
            {
                if (zemberek.kelimeDenetle(word))
                {
                    word = zemberek.kelimeCozumle(word)[0].kok().icerik();
                }
                return word;
            }
            else
            {
                return word;
            }

        }

        public Task<List<string>> SplitWord(string sentence)
        {

            string[] words = sentence.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                bool success = int.TryParse(words[i], out int number);
                if (success)
                {
                    List<int> numbers = nf.findDigitsOfaNumber(number.ToString());
                    foreach (int n in numbers)
                        wordsList.Add(videoFunciton.CreateVideoPath(n.ToString()));
                }
                else
                {
                    if (words[i] != "")
                    {
                        words[i] = FindRootOfTheWord(words[i]);
                        wordsList.Add(videoFunciton.CreateVideoPath(words[i]));
                    }
                }
            }
            return Task.FromResult(wordsList);
        }

        public static bool IsWordInList(List<string> mylist, string word)
        {

            if (mylist.Contains(word))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
