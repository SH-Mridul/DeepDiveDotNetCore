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
                if (context.Request.Method == "GET" && context.Request.Path.StartsWithSegments("/employees") && context.Request.Query.Any())
                {
                    if (context.Request.Query.ContainsKey("id"))
                    {
                        int employeeId;
                        var id = int.TryParse(context.Request.Query["id"],out employeeId);
                        if (EmployeesRepository.CheckEmployeeExist(employeeId))
                        {
                            var employee = EmployeesRepository.GetEmployeeById(employeeId);
                            await context.Response.WriteAsync($"id:{employee.Id}|name:{employee.Name}|salary:{employee.Salary}");
                        }
                        else
                        {
                            await context.Response.WriteAsync($"Not found any employee by this id!");
                        }
                    }
                    else
                    {
                        await context.Response.WriteAsync($"Bad Request!");
                    }
                }
                else if (context.Request.Method == "GET")
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
                            await context.Response.WriteAsync($"Id:{employee.Id}|Name:{employee.Name}|Salary:{employee.Salary}\n");
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
                else if (context.Request.Method == "PUT")
                {
                    if (context.Request.Path.StartsWithSegments("/updateEmployee"))
                    {
                        using var reader = new StreamReader(context.Request.Body);
                        var body = await reader.ReadToEndAsync();
                        var employee = JsonSerializer.Deserialize<Employee>(body);
                        var result = EmployeesRepository.UpdateEmployee(employee);
                        if (result)
                            await context.Response.WriteAsync("Employee is updated!");
                        else
                            await context.Response.WriteAsync("Employee not found!");

                    }
                }
                else if(context.Request.Method == "DELETE" && context.Request.Path.StartsWithSegments("/employees") && context.Request.Query.Any())
                {
                    if(context.Request.Query.ContainsKey("id"))
                    {

                        int employeeId;
                        var id = int.TryParse(context.Request.Query["id"], out employeeId);
                        if(EmployeesRepository.CheckEmployeeExist(employeeId))
                        {
                            var employee = EmployeesRepository.GetEmployeeById(employeeId);
                            var result = EmployeesRepository.Remove(employee);
                            if (result)
                                await context.Response.WriteAsync("Employee removed successfully!");
                            else
                                await context.Response.WriteAsync("something went wrong when employee removing!");
                        }
                        else
                        {
                            await context.Response.WriteAsync("employee not exist by this id!");
                        }
                    }
                    else
                    {
                        await context.Response.WriteAsync("Bad Request!");
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

        public static bool UpdateEmployee(Employee? employee)
        {
            if (employee is not null)
            {
                var oldEmployee = _employees.FirstOrDefault(e => e.Id == employee.Id);
                if (oldEmployee is not null)
                {
                    oldEmployee.Salary = employee.Salary;
                    oldEmployee.Name = employee.Name;
                    return true;
                }
            }

            return false;
        }

        public static Employee GetEmployeeById(int id)
        {
            return _employees.FirstOrDefault( e => e.Id == id);
        }

        public static bool CheckEmployeeExist(int id)
        {
            return _employees.Exists(x => x.Id == id);
        }

        public static bool Remove(Employee employee)
        {
            if (_employees.Remove(employee))
                return true;
            return false;   
            
        }

    }
}
