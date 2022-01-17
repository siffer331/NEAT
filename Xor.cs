using Godot;
using System;
using NEAT;

public class Xor : Node2D {

    public override void _Ready() {
		double[][] data = new double[][] {new double[] {0, 0}, new double[] {0, 1}, new double[] {1, 0}, new double[] {1, 1}};
		double[] res = new double[] {0, 1, 1, 0};
		Population population = new Population(2, 1, 10);
		for(int i = 0; i < 400; i++) {
			foreach(Phenotype phenotype in population.networks) {
				float fitness = 0f;
				for(int j = 0; j < 4; j++) {
					double[] result = phenotype.Propagate(data[j]);
					double distance = Math.Abs(result[0]-res[j]);
					if(distance <= 0.5) fitness += 2f-(float)distance;
				}
				GD.Print(fitness);
				phenotype.fitness = fitness;
			}
			population.MakeNewGeneration();
			GD.Print(" ");
		}
    }

}
