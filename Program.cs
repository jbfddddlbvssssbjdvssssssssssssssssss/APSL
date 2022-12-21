using Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CRUD
{
    class Car : DB_Element<Car>
    {
        public string plate;
        public string color;
        public int speed;

        public Car() { }

        public Car(string plate, string color, int speed)
        {
            this.plate = plate;
            this.color = color;
            this.speed = speed;
        }

        public Car asObject(DataRow row) => new Car(row[0].ToString(),row[1].ToString(),Convert.ToInt32(row[2]));

        public string asString() => $"'{plate}','{color}',{speed}";

        public override string ToString() => $"[{plate}],{color},{speed} km/h";
    }

    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, string> settings = new Dictionary<string, string>()
            {
                { "db_name", "Testing" },
                { "server", "DESKTOP-AJ9MC4H" }
            };

            DB_MySQL controller = new DB_MySQL();
            KeyValuePair<SqlConnection,string> result = controller.connect(settings);
            controller.SetConnection = result.Key;

            if(result.Key == null)
            {
                Console.WriteLine(result.Value);
                return;
            }

            controller.setQuery("CREATE TABLE Cars(Plate CHAR(8) PRIMARY KEY NOT NULL,Color VARCHAR(50),Speed INT)");

            controller.insert<Car>("Cars", new List<Car>() { 
                new Car("AB1231FF","Black",200),
                new Car("CF4441KH","Yellow",250),
                new Car("JJ3765OM","Black",290),
                new Car("OO9090SZ","Gray",190),
                new Car("CF0327QD","White",230),
            });

            controller.update<Car>("Cars", (x) => true, (x) => new Car(x.plate, x.color, x.speed * 2));
            controller.remove<Car>("Cars", (x) => x.speed > 1000);
            controller.select<Car>("Cars", (x) => true).ShowTabs();

        }
    }
}
