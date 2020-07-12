using System.Collections.Generic;
using avaTodo.Models;
using Newtonsoft.Json;
using System.IO;

namespace avaTodo.Services
{
    public class Database
    {

        private List<TodoItem> ToDos = null;

        public IEnumerable<TodoItem> GetItems()
        {

            if (ToDos == null) {
            string content = File.ReadAllText("./todos.json");
            ToDos = JsonConvert.DeserializeObject<List<TodoItem>>(content);
             
            }
            return ToDos;
        }

        public void RemoveItem(TodoItem todo) {
            ToDos.Remove(todo);
            SaveTodos();
        }

        public void SaveTodos() {
            string content = JsonConvert.SerializeObject(ToDos);
            File.WriteAllText("./todos.json", content);
        }
    }
}