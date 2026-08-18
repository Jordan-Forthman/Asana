using Asana.Library.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Asana.API.Database
{
    public class ToDoFilebase
    {
        private static ToDoFilebase? _instance;


        public static ToDoFilebase Current
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new ToDoFilebase();
                }

                return _instance;
            }
        }

        // Storage locations come from FileStorage, configured at startup.
        private ToDoFilebase()
        {
        }

        public int LastKey
        {
            get
            {
                if (ToDos.Any())
                {
                    return ToDos.Select(x => x.Id).Max();
                }
                return 0;
            }
        }

        public ToDo AddOrUpdate(ToDo toDo)
        {
            //set up a new Id if one doesn't already exist
            if(toDo.Id <= 0)
            {
                toDo.Id = LastKey + 1;
            }

            //go to the right place
            string path = Path.Combine(FileStorage.ToDoRoot, $"{toDo.Id}.json");
            

            //if the item has been previously persisted
            if(File.Exists(path))
            {
                //blow it up
                File.Delete(path);
            }

            //write the file
            File.WriteAllText(path, JsonConvert.SerializeObject(toDo));

            //return the item, which now has an id
            return toDo;
        }
        
        public List<ToDo> ToDos
        {
            get
            {
                var root = new DirectoryInfo(FileStorage.ToDoRoot);
                var _toDos = new List<ToDo>();
                foreach(var toDoFile in root.GetFiles())
                {
                    var toDo = JsonConvert
                        .DeserializeObject<ToDo>
                        (File.ReadAllText(toDoFile.FullName));
                    if(toDo != null)
                    {
                        _toDos.Add(toDo);
                    }

                }
                return _toDos;
            }
        }
        public bool Delete(string type, string id)
        {
            if (int.TryParse(id, out int idInt) && type == "ToDo")
            {
                string path = Path.Combine(FileStorage.ToDoRoot, $"{idInt}.json");
                if (File.Exists(path))
                {
                    File.Delete(path);
                    return true;
                }
            }
            return false;
        }
    }


   
}