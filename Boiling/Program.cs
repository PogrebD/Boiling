using Boiling.DirectSolver;
using Boiling.FiniteElement.Time;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using SharpMath.EquationsSystem.Preconditions;
using SharpMath.EquationsSystem.Solver;
using SharpMath.FiniteElement;
using SharpMath.FiniteElement._2D.Parameters;
using SharpMath.FiniteElement.Materials.Boiling;
using SharpMath.FiniteElement.Materials.HarmonicWithoutChi;
using SharpMath.FiniteElement.Materials.MaterialSetter.Areas;
using SharpMath.FiniteElement.Materials.Providers;
using SharpMath.Geometry;
using SharpMath.Geometry._2D;
using SharpMath.Geometry.Splitting;
using SharpMath.Matrices.Sparse;
using SharpMath.Vectors;

void ConfigureServices(IServiceCollection services)
{
    IConfiguration configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();
    services.AddSingleton(configuration);

    services.AddScoped<LocalOptimalSchemeConfig>(provider =>
    {
        provider.GetService<IConfiguration>();
        var losConfig = configuration
            .GetSection("Boiling")
            .GetSection("LOS")
            .Get<LocalOptimalSchemeConfig>();

        return losConfig!;
    });

    services.AddTransient<BoilingDirectSolver>();
    services.AddTransient<LUSparseThroughProfileConversion>();

    services.AddTransient<ISLAESolver<SparseMatrix>, LocalOptimalScheme>();
    // services.AddTransient<ISLAESolver<SparseMatrix>, LUSparseThroughProfileConversion>();

    services.AddTransient<LUPreconditioner>();
    services.AddTransient<SparsePartialLUResolver>();

    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(configuration)
        .Enrich.FromLogContext()
        .CreateLogger();
    services.AddLogging(loggingBuilder =>
        loggingBuilder.AddSerilog(dispose: true));
}

void RunBoiling()
{
    var services = new ServiceCollection();
    ConfigureServices(services);
    var provider = services.BuildServiceProvider();

    var logger = provider.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Boiling, You're just a miserable copy of me!");
    logger.LogCritical("No, I'm the upgrade!");

    const double r = 0.07; //радиус
    const double h = 0.08; //высота(z)

    var water = new RectArea(
        new Rectangle(
            0, 0,
            r, h
        ),
        materialId: 0
    );

    var areas = new AreasMaterialSetterFactory(
        [water],
        defaultMaterialIdId: 0
    );

    const int nestingDegree = 1;

    var grid = new GridBuilder()
        .SetXAxis(new AxisSplitParameter(
            [0, r],
            new UniformSplitter(70 * nestingDegree)
        ))
        .SetYAxis(new AxisSplitParameter(
            [0, h],
            new UniformSplitter(80 * nestingDegree)
        ))
        .SetMaterialSetterFactory(areas)
        .Build();


    //картинка направление скорости
    /*var velocityParameter = new ConvectionVelocity(grid.Nodes, 0.1);
    for (var i = 0; i < grid.Nodes.TotalPoints; i++)
    {
        var velocity = velocityParameter.Get(grid.Nodes[i]);
        Console.WriteLine($"{grid.Nodes[i].X:F5} {grid.Nodes[i].Y:F5} {velocity.X:E5} {velocity.Y:E5}");
    }*/


    /*var filePath = $@"C:\Users\dimap\RiderProjects\BoilingTrue\View\View\velocity.txt";

    var velocityParameter2 = new ConvectionVelocity(grid.Nodes, 0.001);
    using (var writer = new StreamWriter(filePath))
    {
        for (var i = 0; i < grid.Nodes.TotalPoints; i++)
        {
            var velocity = velocityParameter2.Get(grid.Nodes[i]);
            writer.WriteLine($"{grid.Nodes[i].X:F5} {grid.Nodes[i].Y:F5} {velocity.X:E5} {velocity.Y:E5}");
        }
    }*/

    var materialProvider = new BoilingMaterialProvider([
        new BoilingMaterial(0.6, 999.97, 4200d)
    ]);

    int timeSpl = 1000;

    var solver = provider.GetRequiredService<BoilingDirectSolver>();
    solver.Allocate(grid);
    solver.Allocate(materialProvider);
    solver.Allocate(new UniformSplitter(1 * timeSpl)
        .EnumerateValues(new Interval(0d, (double)timeSpl))
        .ToArray());

    var femSolution = solver.Solve(Vector.Create(grid.Nodes.TotalPoints, 25));


    /*var filePath = $@"C:\Users\dimap\RiderProjects\Boiling\View\View\velocity{velocitytxt}\data{timeSpl}.txt";

    using (var writer = new StreamWriter(filePath))
    {
        for (var i = 0; i < grid.Nodes.TotalPoints; i++)
        {
            var u = femSolution.Calculate(grid.Nodes[i], (double)timeSpl);
            writer.WriteLine($"{grid.Nodes[i].X:F5} {grid.Nodes[i].Y:F5} {u:E5}");
        }
    }*/
    Console.WriteLine("super");
}

RunBoiling();