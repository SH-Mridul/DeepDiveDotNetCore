using System.Text.Json;

namespace DeepDiveWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            app.Run(async (HttpContext context) =>
            {
                if(context.Request.Method == "GET")
                {
                    if (context.Request.Path.StartsWithSegments("/"))
                    {
                        await context.Response.WriteAsync($"The method is :{context.Request.Method}\n");
                        await context.Response.WriteAsync($"The Url path is :{context.Request.Path}\n");

                        await context.Response.WriteAsync($"----------------------Headers--------------------------\n");
                        foreach (var key in context.Request.Headers.Keys)
                        {
                            await context.Response.WriteAsync($"{key}:{context.Request.Headers[key]}\n");
                        }
                    }
                    else if (context.Request.Path.StartsWithSegments("/employees"))
                    {
                        var employees = EmployeesRepository.GetEmployees();
                        foreach (var employee in employees)
                        {
                            await context.Response.WriteAsync($"{employee.Name}:{employee.Salary}\n");
                        }
                    }
                }
                else if (context.Request.Method == "POST")
                {
                    if (context.Request.Path.StartsWithSegments("/employees"))
                    {
                        using var reader = new StreamReader(context.Request.Body);
                        var body = await reader.ReadToEndAsync();
                        var employee = JsonSerializer.Deserialize<Employee>(body);
                        EmployeesRepository.AddEmployee(employee);
                    }
                }
            });

            app.Run();
        }
    }


    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Salary { get; set; }

        public Employee(int id,string name,double salary)
        {
           Id = id;
           Name = name;
           Salary = salary;
        }
    }


    public static class EmployeesRepository
    {
        private static List<Employee> _employees = new List<Employee>
        {
            new Employee(1, "Alice", 50000),
            new Employee(2, "Bob", 60000),
            new Employee(3, "Charlie", 55000),
            new Employee(4, "Diana", 70000)
        };

        public static List<Employee> GetEmployees() => _employees;
        public static void AddEmployee(Employee? employee){
            if (employee is not null)
                _employees.Add(employee);
        }

    }
}
