//Inspired by https://github.com/b2developer/MonopolyNEAT/tree/main/Monopoly/NeuroEvolution
using System;
using System.Collections.Generic;
using Godot;


namespace NEAT {

	public class Species {

		public List<Genotype> networks = new List<Genotype>();
		public float bestFitness = 0;
		public float totalFitness = 0;
		public int staleTime = 0;

		public Species(Genotype genotype) {
			networks.Add(genotype);
		}

		public void UpdateStaleness() {
			float best = 0f;
			foreach(Genotype genotype in networks) best = Math.Max(best, genotype.fitness);
			if(best > bestFitness) {
				staleTime = 0;
				bestFitness = best;
			}
			else staleTime++;
		}

		public void SortByFitness() {
			networks.Sort((Genotype a, Genotype b) => (int)((a.fitness - b.fitness)*10f));
		}

		public void KeepTop(float part) {
			int top = (int)Math.Ceiling(networks.Count*part);
			networks.RemoveRange(top, networks.Count - top);
		}

		public void Reset() {
			networks.RemoveRange(1, networks.Count-1);
		}

		public void CalculateAdjustedFitness(Crossover crossover) {
			totalFitness = 0f;
			for(int i = 0; i < networks.Count; i++) {
				float alike = 0;
				for(int j = 0; j < networks.Count; j++) {
					if(i == j) continue;
					if(crossover.Alike(networks[i], networks[j])) alike++;
				}
				networks[i].adjustedFitness = networks[i].fitness/alike;
				totalFitness += networks[i].adjustedFitness;
			}
		}

		public Genotype MakeOffspring(Crossover crossover, Mutation mutation, int inputs, int outputs) {
			Random random = new Random();
			Genotype genotype;
			if((float)random.NextDouble() < crossover.crossoverChance) {
				int parent1 = GetRandom(random);
				int parent2 = GetRandom(random);
				if(networks[parent1].fitness < networks[parent2].fitness) {
					int temp = parent1;
					parent1 = parent2;
					parent2 = temp;
				}
				genotype = crossover.MakeOffspring(networks[parent1], networks[parent2], random);
			}
			else genotype = networks[GetRandom(random)].Clone();
			mutation.Mutate(genotype, inputs, outputs);
			return genotype;
		}

		public int GetRandom(Random random) {
			float total = 0f;
			foreach(Genotype genotype in networks) total += genotype.fitness;
			float selected = ((float)random.NextDouble())*total;
			float sum = 0f;
			for(int i = 0; i < networks.Count; i++) {
				sum += networks[i].fitness;
				if(sum > selected) return i;
			}
			return 0;
		}

	}

	public class Population {
		
		public int populationSize;
		public int inputs;
		public int outputs;

		public int maxStaleness = 15;
		public float speciesTop = 0.2f;

		public Crossover crossover = new Crossover();
		public Mutation mutation = new Mutation();

		public int generation = 0;

		private List<Genotype> population;
		public List<Phenotype> networks;
		private List<Species> species;

		public Population(int inputs, int outputs, int populationSize) {
			this.populationSize = populationSize;
			this.inputs = inputs;
			this.outputs = outputs;
			mutation.nodeIds = inputs + outputs;
			species = new List<Species>();
			population = new List<Genotype>();
			networks = new List<Phenotype>();
			GenerateInitialGeneration();
			CreateNetworks();
		}

		public void GenerateInitialGeneration() {
			population = new List<Genotype>();
			for(int i = 0; i < populationSize; i++) population.Add(new Genotype(inputs, outputs));
			species = new List<Species>();
			foreach(Genotype network in population) AddToSpecies(network);
		}

		public void MakeNewGeneration() {
			for(int i = 0; i < populationSize; i++) population[i].fitness = networks[i].fitness;
			float totalFitness = 0f;
			for(int i= 0; i < species.Count; i++) {
				species[i].CalculateAdjustedFitness(crossover);
				totalFitness += species[i].totalFitness;
				species[i].UpdateStaleness();
				species[i].SortByFitness();
				species[i].KeepTop(speciesTop);
				if(species[i].staleTime > maxStaleness || species[i].networks.Count == 1) species.RemoveAt(i);
			}
			List<Genotype> newPopulation = new List<Genotype>();
			foreach(Species specie in species) {
				int make = (int)(populationSize*specie.totalFitness/totalFitness) - 1;
				while(make-- > 0) {
					newPopulation.Add(specie.MakeOffspring(crossover, mutation, inputs, outputs));
				}
			}
			Random random = new Random();
			for(int i = species.Count + newPopulation.Count; i < populationSize; i++)
				newPopulation.Add(species[random.Next(species.Count)].MakeOffspring(crossover, mutation, inputs, outputs));
			foreach(Species specie in species) specie.Reset();
			foreach(Genotype genotype in newPopulation) AddToSpecies(genotype);
			foreach(Species specie in species) newPopulation.Add(specie.networks[0]);
			population = newPopulation;
			CreateNetworks();
			generation++;
		}

		public void CreateNetworks() {
			networks = new List<Phenotype>();
			for(int i = 0; i < populationSize; i++) networks.Add(new Phenotype(inputs, outputs, population[i]));
		}

		public void AddToSpecies(Genotype genotype) {
			bool found = false;
			for(int i = 0; i < species.Count; i++) {
				if(crossover.Alike(species[i].networks[0], genotype)) {
					found = true;
					species[i].networks.Add(genotype);
					break;
				}
			}
			if(!found) species.Add(new Species(genotype));
		}


	}


}