<h1>Camel Up Probability Calculator</h1>
<p>
  This is a custom-built tool for calculating the probability of each camel winning the current leg in the board game <strong>Camel Up</strong>. It uses a full simulation approach to evaluate every possible outcome based on the current board setup and remaining dice.
</p>

![image](https://github.com/user-attachments/assets/12bfab15-1e7c-4ffd-adbc-157cbcaa4a7c)

<h2>What Is Camel Up?</h2>

<p>
  <strong>🎲 About the Game:</strong><br>
  <em>Camel Up</em>: up to eight players bet on five racing camels, trying to suss out which ones will place first and second in a quick race around a pyramid. The earlier you place your bet, the more you can win — should you guess correctly, of course. Camels don't run neatly, however, sometimes landing on top of another one and being carried toward the finish line. Who's going to run when? That all depends on how the dice come out of the pyramid dice shaker, which releases one die at a time when players pause from their bets long enough to see who's actually moving!

This 2018 edition of Camel Up features new artwork, a new game board design, a new pyramid design, engraved dice, and new game modes, including crazy rogue camels that start the race running in the opposite direction! You never know how a race will end!
Taken from:  (https://boardgamegeek.com/boardgame/260605/camel-up-second-edition)
</p>

<p>
  <strong>📊 About This Project:</strong><br>
  This tool simulates <em>every possible dice roll order</em> and <em>every value</em> those dice could produce, so that you can easily see which camel is likely to win the leg. It was developed in C# using MVC patterns and recursion-heavy logic, and was made as a learning project.
</p>

<h2>Tech Stack</h2>
<ul>
  <li><strong>.NET Core</strong> (C# backend logic)</li>
  <li><strong>ASP.NET MVC</strong> (Web application framework)</li>
  <li><strong>Razor Pages</strong> (UI templating)</li>
  <li><strong>Bootstrap & JS</strong> (Frontend layout)</li>
</ul>

<h2>Project Structure</h2>
<pre><code>
CamelUpProbabilityCalc/
├── Controllers/         # Web controllers (HomeController, etc.)
├── Logic/               # Probability engine and simulation logic
├── Models/              # Game state, dice roll, and camel info
├── Views/               # Razor views for UI rendering
├── wwwroot/             # Static assets like CSS/JS
└── Program.cs           # Application startup
</code></pre>

<h2>How to Use The Calculator</h2>
<ol>
  <li>
    <strong>Clone the repo</strong>
    <pre><code>git clone https://github.com/your-username/CamelUpProbabilityCalc.git
cd CamelUpProbabilityCalc</code></pre>
  </li>
  <li>
    <strong>Run in Visual Studio</strong><br>
    Open the solution and build/run to launch the web app.
  </li>
  <li>
    <strong>Set up your game state:</strong>!

    <ul>
      <li>Place camels where they currently are on the board.</li>
      <li>Mark which camel dice have already been rolled this leg.</li>
    </ul>
  </li>
  <li>
    The side panel will automatically populate once calculations have been made.
  </li>
</ol>

<h2>How It Works</h2>
<p>
  The core simulation engine performs the following steps:
</p>
<ul>
  <li>Generates all permutations of unused dice colors.</li>
  <li>Generates all value combinations for those dice (1, 2, or 3).</li>
  <li>Clones the board state at every step to ensure each branch is isolated.</li>
  <li>Simulates camel movement using full stack-carrying logic per Camel Up rules.</li>
  <li>Tallies up which camel ends up in the lead in each simulation path.</li>
</ul>

<h2>Current Limitations</h2>
<ul>
  <li>This only simulates a single leg, and there is no logic for ending the race once camels are around space 16.</li>
  <li>Crazy camels and spectator tiles are not implemented yet.</li>
</ul>
