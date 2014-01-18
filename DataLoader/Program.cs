using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneWordStory.Concrete;
using OneWordStory.Domain.Concrete;
using OneWordStory.Domain.Entities;
using Extensions;

namespace DataLoader
{
    class Program
    {


        static List<string> users = new List<string>() { "users/705", "users/737", "users/769", "users/801", "users/833", "users/834", "users/835", "users/836", "users/837", "users/838", "users/839", "users/840" };


        static void Main(string[] args)
        {

            var story = File.ReadLines("../../Story.txt");

            string storyID = "";

            foreach (var paragraph in story)
            {

                bool newParagraph = true;
                if (!string.IsNullOrEmpty(paragraph))
                {

                    Console.WriteLine("Saving Paragraph");
                    var words = paragraph.Split(' ');

                    foreach (string word in words)
                    {
                        if (!string.IsNullOrEmpty(word))
                        {
                            Console.WriteLine("Saving word");
                            StoryRepository repository = new StoryRepository();
                            var result = repository.AddWord(storyID, word, users.PickRandom<string>(), newParagraph);
                            newParagraph = false;
                            storyID = result.Story.Id;
                        }
                    }
                }
            }

        }

        



    }
}

