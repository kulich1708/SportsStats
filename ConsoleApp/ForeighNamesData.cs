using ConsoleApp;
using System.Collections.Generic;

public class ForeignNamesData : INamesData
{
	public List<string> FirstNames => new List<string>
	{
		"James", "John", "Robert", "Michael", "William", "David", "Richard", "Joseph",
		"Thomas", "Charles", "Christopher", "Daniel", "Matthew", "Anthony", "Donald", "Mark",
		"Paul", "Steven", "Andrew", "Kenneth", "Joshua", "Kevin", "Brian", "George",
		"Edward", "Ronald", "Timothy", "Jason", "Jeffrey", "Ryan", "Jacob", "Gary",
		"Nicholas", "Eric", "Jonathan", "Stephen", "Larry", "Justin", "Scott", "Brandon"
	};

	public List<string> LastNames => new List<string>
	{
		"Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis",
		"Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas",
		"Taylor", "Moore", "Jackson", "Martin", "Lee", "Perez", "Thompson", "White",
		"Harris", "Sanchez", "Clark", "Ramirez", "Lewis", "Robinson", "Walker", "Young",
		"Allen", "King", "Wright", "Scott", "Torres", "Nguyen", "Hill", "Flores",
		"Green", "Adams", "Nelson", "Baker", "Hall", "Rivera", "Campbell", "Mitchell",
		"Carter", "Roberts", "Gomez", "Phillips", "Evans", "Turner", "Diaz", "Parker",
		"Cruz", "Edwards", "Collins", "Reyes", "Stewart", "Morris", "Morales", "Murphy",
		"Cook", "Rogers", "Gutierrez", "Ortiz", "Morgan", "Cooper", "Peterson", "Bailey",
		"Reed", "Kelly", "Howard", "Ramos", "Kim", "Cox", "Ward", "Richardson",
		"Watson", "Brooks", "Chavez", "Wood", "James", "Bennett", "Gray", "Mendoza",
		"Ruiz", "Hughes", "Price", "Alvarez", "Castillo", "Sanders", "Patel", "Myers",
		"Long", "Ross", "Foster", "Jimenez", "Powell", "Jenkins", "Perry", "Russell",
		"Sullivan", "Bell", "Coleman", "Butler", "Henderson", "Barnes", "Gonzales", "Fisher",
		"Vasquez", "Simmons", "Romero", "Jordan", "Patterson", "Alexander", "Hamilton", "Graham",
		"Reynolds", "Griffin", "Wallace", "Moreno", "West", "Cole", "Hayes", "Bryant",
		"Herrera", "Gibson", "Lawrence", "Ellis", "Marshall", "Berry", "Reid", "Mason"
	};
}