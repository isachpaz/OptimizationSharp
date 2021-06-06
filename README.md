# OptimizationSharp

## Particle Swarm Optimization in C#

Particle Swarm Optimization (PSO) is a computational method for optimization of a problem by simulating a set of moving particles that move around a search-space. Now let's say that you want to find the (global) minimum of the square  function: f(x) = x[0]^2 + x[1]^2.


     var solverConfig = PSOSolverConfig.CreateDefault(
                numberParticles: 500,
                maxEpochs: 1000,
                lowerBound: new double[] {-20, -20},
                upperBound: new double[] {10, 10},
                isStoppingCriteriaEnabled: false); // If not enabled, the optimization, will run over the total number of epochs (maxEpochs).
                
      Func<double[], double> func = x => (x[0] * x[0]) + (x[1] * x[1]);
      var solver = new ParticleSwarmMinimization(func, solverConfig);

      solver.OnAfterEpoch += (s, d) =>
            {
                Console.WriteLine($"Iteration: {d.Iteration}\n" +
                                  $"Best fitness: {d.BestFitness:##.000} \n" +
                                  $"Best global position: " +
                                  $"x1: {d.BestPosition[0]:##.000}" +
                                  $"x2: {d.BestPosition[1]:##.000}");
            };

       var solution = solver.Solve();
       Assert.AreEqual(0, solution.BestPosition[0], 1E-6);
       Assert.AreEqual(0, solution.BestPosition[1], 1E-6);
       
