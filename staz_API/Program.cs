using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;
using System.Xml.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapPost("/birthday/{PESEL}", (string PESEL) =>
{
    if (CheckPESEL(PESEL))
    {
        string bDay = PESEL.Substring(0, 2) + "/" + PESEL.Substring(2, 2) + "/" + PESEL.Substring(4, 2);
        bDay = DateTime.ParseExact(bDay, "yy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("yyy-MM-dd");
        var birthday = DateTime.Parse(bDay);
        int age = (int)((DateTime.Now - birthday).TotalDays / 365.242199);
        return age.ToString() + " years old.";
    }
    else return "Wrong PESEL number!";
});

app.MapPost("/promotion/{PESEL}", (string PESEL) =>
{
    if (CheckPESEL(PESEL))
    {
        int bDay = Int16.Parse(PESEL.Substring(4, 2));
        int bMonth = Int16.Parse(PESEL.Substring(2, 2));

        int tDay = DateTime.Now.Day;
        int tMonth = DateTime.Now.Month;
        if (bDay == tDay && bMonth == tMonth)
        {
            return "10%";
        }
        else if (bMonth == tMonth)
        {
            return "5%";
        }
        else if (tMonth >= 2 && tMonth <= 9)
        {
            return "0%";
        }
        else return "2,5%";
    }
    else return "Wrong PESEL number!";
});

app.MapPost("/wishes/{PESEL}/{Name}/{Surname}", (string PESEL, string Name, string Surname) =>
{
    if (CheckPESEL(PESEL))
    {
        int sex = Int16.Parse(PESEL.Substring(9, 1));
        if (sex % 2 == 0)
        {
            return String.Format("Klientko {0} {1}! ¯yczymy Ci sto lat!", Name, Surname);
        }
        else return String.Format("Kliencie {0} {1}! ¯yczymy Ci sto lat!", Name, Surname);
    }
    else return "Wrong PESEL number!";
});

bool CheckPESEL(string PESEL)
{
    bool isNumeric = ulong.TryParse(PESEL, out _);
    if (PESEL.Length == 11 && isNumeric)
    {
        int[] controlNumbers = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };
        int sum = 0;
        for(int i = 0; i<10; i++)
        {
            int a = Int16.Parse(PESEL.Substring(i, 1));
            if (a * controlNumbers[i] >10)
            {
                sum += Int16.Parse((a * controlNumbers[i]).ToString().Substring(1, 1));
            }else sum+=a*controlNumbers[i];
        }
        if (sum>10)
        {
            sum = Int16.Parse((sum).ToString().Substring(1, 1));
        }
        if (10-sum == Int16.Parse(PESEL.Substring(10, 1)))
        {
            return true;
        }
        return false;
    }
    else return false;
}


app.Run();