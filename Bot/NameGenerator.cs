using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot
{
    public class NameGenerator
    {
        List<string> pre = new List<string>();
        List<string> mid = new List<string>();
        List<string> sur = new List<string>();

        private static char[] Vowels = { 'a', 'e', 'i', 'o', 'u', 'ä', 'ö', 'õ', 'ü', 'y' };
        private static char[] Consonants = { 'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'y' };
        Random rnd = new Random();
        private string fileName;

        /**
         * Create new random name generator object. refresh() is automatically called.
         * @param fileName insert file name, where syllables are located
         * @throws IOException
         */
        public NameGenerator(string fileName)
        {
            this.fileName = fileName;
            Refresh();
        }

        public NameGenerator(TextReader reader)
        {
            Load(reader);
        }

        /**
         * Change the file. refresh() is automatically called during the process.
         * @param fileName insert the file name, where syllables are located.
         * @throws IOException
         */
        public void changeFile(string fileName)
        {
            if (fileName == null) throw new ArgumentNullException("fileName");
            this.fileName = fileName;
            Refresh();
        }

        /**
         * Refresh names from file. No need to call that method, if you are not changing the file during the operation of program, as this method
         * is called every time file name is changed or new NameGenerator object created.
         * @throws IOException
         */
        public void Refresh()
        {
            if (fileName == null) return;

            using (var reader = new StreamReader(fileName))
            {
                Load(reader);
            }
        }

        private void Load(TextReader reader)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Length > 0)
                {
                    if (line[0] == '-')
                        pre.Add(line.Substring(1).ToLower());
                    else if (line[0] == '+')
                        sur.Add(line.Substring(1).ToLower());
                    else
                        mid.Add(line.ToLower());
                }
            }
        }

        private string upper(string s)
        {
            return s.Substring(0, 1).ToUpper() + s.Substring(1);
        }

        private bool containsConsFirst(List<string> array)
        {
            foreach (string s in array)
            {
                if (ConsonantFirst(s)) return true;
            }
            return false;
        }

        private bool containsVocFirst(List<string> array)
        {
            foreach (string s in array)
            {
                if (VowelFirst(s)) return true;
            }
            return false;
        }

        private bool allowCons(List<string> array)
        {
            foreach (string s in array)
            {
                if (hatesPreviousVowels(s) || !hatesPreviousConsonants(s)) return true;
            }
            return false;
        }

        private bool allowVocs(List<string> array)
        {
            foreach (string s in array)
            {
                if (hatesPreviousConsonants(s) || hatesPreviousVowels(s) == false) return true;
            }
            return false;
        }

        private bool expectsVowel(string s)
        {
            if (s.Substring(1).Contains("+v")) return true;
            else return false;
        }
        private bool expectsConsonant(string s)
        {
            if (s.Substring(1).Contains("+c")) return true;
            else return false;
        }
        private bool hatesPreviousVowels(string s)
        {
            if (s.Substring(1).Contains("-c")) return true;
            else return false;
        }
        private bool hatesPreviousConsonants(string s)
        {
            if (s.Substring(1).Contains("-v")) return true;
            else return false;
        }

        private string PureSyl(string s)
        {
            s = s.Trim();
            if (s[0] == '+' || s[0] == '-') s = s.Substring(1);
            return s.Split(' ')[0];
        }

        private bool VowelFirst(string s)
        {
            return Vowels.Contains(char.ToLower(s[0]));
        }

        private bool ConsonantFirst(string s)
        {
            return Consonants.Contains(char.ToLower(s[0]));
        }

        private bool VowelLast(string s)
        {
            return Vowels.Contains(char.ToLower(s[s.Length - 1]));
        }

        private bool ConsonantLast(string s)
        {
            return Consonants.Contains(char.ToLower(s[s.Length - 1]));
        }


        /**
         * Compose a new name.
         * @param syls The number of syllables used in name.
         * @return Returns composed name as a string
         * @throws ApplicationException when logical mistakes are detected inside chosen file, and program is unable to complete the name.
         */
        public string Compose(int syls)
        {
            if (syls > 2 && mid.Count == 0) throw new ApplicationException("You are trying to create a name with more than 3 parts, which requires middle parts, which you have none in the file " + fileName + ". You should add some. Every word, which doesn't have + or - for a prefix is counted as a middle part.");
            if (pre.Count == 0) throw new ApplicationException();
            if (sur.Count == 0) throw new ApplicationException("You have no suffixes to end a name. add some and use " + " prefix, to identify it as a suffix for a name. (example: +asd)");
            if (syls < 1) throw new ApplicationException("compose(int syls) can't have less than 1 syllable");
            int expecting = 0; // 1 for Vowel, 2 for consonant
            int last = 0; // 1 for Vowel, 2 for consonant
            string name;
            int a = (int)(rnd.NextDouble() * pre.Count);

            if (VowelLast(PureSyl(pre[a]))) last = 1;
            else last = 2;

            if (syls > 2)
            {
                if (expectsVowel(pre[a]))
                {
                    expecting = 1;
                    if (containsVocFirst(mid) == false) throw new ApplicationException();
                }
                if (expectsConsonant(pre[a]))
                {
                    expecting = 2;
                    if (containsConsFirst(mid) == false) throw new ApplicationException();
                }
            }
            else
            {
                if (expectsVowel(pre[a]))
                {
                    expecting = 1;
                    if (containsVocFirst(sur) == false) throw new ApplicationException();
                }
                if (expectsConsonant(pre[a]))
                {
                    expecting = 2;
                    if (containsConsFirst(sur) == false) throw new ApplicationException();
                }
            }
            if (VowelLast(PureSyl(pre[a])) && allowVocs(mid) == false) throw new ApplicationException();

            if (ConsonantLast(PureSyl(pre[a])) && allowCons(mid) == false) throw new ApplicationException();

            int[] b = new int[syls];
            for (int i = 0; i < b.Length - 2; i++)
            {

                do
                {
                    b[i] = (int)(rnd.NextDouble() * mid.Count);
                    //System.out.println("exp " +expecting+" VowelF:"+VowelFirst(mid.get(b[i]))+" syl: "+mid.get(b[i]));
                }
                while (expecting == 1 && VowelFirst(PureSyl(mid[b[i]])) == false || expecting == 2 && ConsonantFirst(PureSyl(mid[b[i]])) == false
                        || last == 1 && hatesPreviousVowels(mid[b[i]]) || last == 2 && hatesPreviousConsonants(mid[b[i]]));

                expecting = 0;
                if (expectsVowel(mid[b[i]]))
                {
                    expecting = 1;
                    if (i < b.Length - 3 && containsVocFirst(mid) == false) throw new ApplicationException();
                    if (i == b.Length - 3 && containsVocFirst(sur) == false) throw new ApplicationException();
                }
                if (expectsConsonant(mid[b[i]]))
                {
                    expecting = 2;
                    if (i < b.Length - 3 && containsConsFirst(mid) == false) throw new ApplicationException();
                    if (i == b.Length - 3 && containsConsFirst(sur) == false) throw new ApplicationException();
                }
                if (VowelLast(PureSyl(mid[b[i]])) && allowVocs(mid) == false && syls > 3) throw new ApplicationException();

                if (ConsonantLast(PureSyl(mid[b[i]])) && allowCons(mid) == false && syls > 3) throw new ApplicationException();
                if (i == b.Length - 3)
                {
                    if (VowelLast(PureSyl(mid[b[i]])) && allowVocs(sur) == false) throw new ApplicationException();

                    if (ConsonantLast(PureSyl(mid[b[i]])) && allowCons(sur) == false) throw new ApplicationException();
                }
                if (VowelLast(PureSyl(mid[b[i]]))) last = 1;
                else last = 2;
            }

            int c;
            do
            {
                c = (int)(rnd.NextDouble() * sur.Count);
            }
            while (expecting == 1 && VowelFirst(PureSyl(sur[c])) == false || expecting == 2 && ConsonantFirst(PureSyl(sur[c])) == false
                    || last == 1 && hatesPreviousVowels(sur[c]) || last == 2 && hatesPreviousConsonants(sur[c]));

            name = upper(PureSyl(pre[a].ToLower()));
            for (int i = 0; i < b.Length - 2; i++)
            {
                name += PureSyl(mid[b[i]].ToLower());
            }
            if (syls > 1)
                name += PureSyl(sur[c].ToLower());
            return name;
        }
    }
}
