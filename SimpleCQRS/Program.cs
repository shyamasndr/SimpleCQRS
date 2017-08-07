using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCQRS
{
    public class Person
    {
        private int age;
        EventBroker broker;
        public Person(EventBroker broker)
        {
            this.broker = broker;
            broker.commands += BrokerOnCommands;
            broker.query += BrokerOnQuery;
        }

        private void BrokerOnQuery(object sender, Query e)
        {
            e.Result = this.age;
        }

        private void BrokerOnCommands(object sender, Command e)
        {
            var command= e as ChangeAgeCommand;
            if(command?.Target==this)
            {
                this.age = command.Age;
            }
        }
    }

    public class EventBroker
    {
        public event EventHandler<Command> commands;
        public event EventHandler<Query> query;
        public void Command(Command c)
        {
            commands?.Invoke(this, c);
        }

        public T Query<T>(Query q)
        {
            query?.Invoke(this, q);
            return (T)q.Result;
        }
    }

    public class Query
    {
        public object Result;
    }
    public class AgeQuery:Query
    {
        public readonly Person Target;

        public AgeQuery(Person target)
        {
            Target = target;
        }
    }
    public class Command:EventArgs
    {
    }
    public class ChangeAgeCommand:Command
    {
        public readonly Person Target;
        public readonly int Age;

        public ChangeAgeCommand(Person target, int age)
        {
            Target = target;
            Age = age;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            EventBroker broker = new EventBroker();
            Person person = new Person(broker);
            ChangeAgeCommand changeAgeCommand = new ChangeAgeCommand(person, 3);
            broker.Command(changeAgeCommand);

            Query ageQuery = new AgeQuery(person);
            var age = broker.Query<int>(ageQuery);
            Console.WriteLine("Age Query retrurned ${age}");
            Console.ReadLine();
            
        }
    }
}
