using System;
using System.Collections.Generic;
using Godot;


namespace NEAT {


	public class Mutation {
		public float linkChance = 0.2f;
		public float nodeChance = 0.1f;
		public float enableChance = 0.6f;
		public float disableChance = 0.2f;
		public float weightChance = 2.0f;
		public float shiftChance = 0.9f;
		public float shiftStep = 0.1f;

		public Dictionary<(int, int), int> innovations = new Dictionary<(int, int), int>();
		public Dictionary<int, int> nodeGenerations = new Dictionary<int, int>();
		public int innovation = 0;
		public int nodeIds = 0;

		public Mutation() {
		}

		public int GetInnovation(int inNode, int outNode) {
			if(innovations.ContainsKey((inNode, outNode))) return innovations[(inNode, outNode)];
			innovation++;
			innovations[(inNode, outNode)] = innovation;
			return innovation;
		}

		public void Mutate(Genotype genotype, int inputs, int outputs) {
			Random random = new Random();
			float p = weightChance;
			while(random.NextDouble() < p--) MutateWeight(genotype, random);
			p = nodeChance;
			while(random.NextDouble() < p--) MutateNode(genotype, random);
			p = linkChance;
			while(random.NextDouble() < p--) MutateLink(genotype, random, inputs, outputs);
			p = disableChance;
			while(random.NextDouble() < p--) MutateDisable(genotype, random);
			p = enableChance;
			while(random.NextDouble() < p--) MutateEnable(genotype, random);
		}

		public void MutateLink(Genotype genotype, Random random, int inputs, int outputs) {
			List<(int, int)> candidates = new List<(int, int)>();
			int to = -1;
			int from = -1;
			for(int i = 0; i < genotype.nodes.Count; i++) {
				for(int j = i+1; j < genotype.nodes.Count; j++) {
					if(i < inputs && j < inputs) continue;
					if(i >= inputs && i < inputs+outputs && j >= inputs && j < inputs+outputs) continue;
					from = genotype.nodes[i];
					to = genotype.nodes[j];
					bool found = false;
					foreach(EdgeGene edge in genotype.genes) {
						if((edge.inNode == from && edge.outNode == to) || (edge.inNode == to && edge.outNode == from)) {
							found = true;
							break;
						}
					}
					if(!found) candidates.Add((to, from));
				}
			}
			if(candidates.Count == 0) return;
			(to, from) = candidates[random.Next(candidates.Count)];
			
			Dictionary<int, List<int>> graph = new Dictionary<int, List<int>>();
			graph[to] = new List<int>(); 
			foreach(EdgeGene edge in genotype.genes) {
				if(!graph.ContainsKey(edge.inNode)) graph[edge.inNode] = new List<int>();
				graph[edge.inNode].Add(edge.outNode);
			}

			bool Find(int node) {
				if(node == from) return true;
				for(int i = 0; i < graph[node].Count; i++) {
					if(Find(graph[node][i])) return true;
				}
				return false;
			}

			if(Find(to) || (from >= inputs && from < inputs+outputs) || to < inputs) {
				int temp = to;
				to = from;
				from = temp;
			}
			genotype.genes.Add(new EdgeGene(from, to, random.NextDouble()*4-2, GetInnovation(from, to)));
		}

		public void MutateNode(Genotype genotype, Random random) {
			List<int> candidates = new List<int>();
			for(int i = 0; i < genotype.genes.Count; i++) {
				if(genotype.genes[i].enabled) candidates.Add(i);
			}
			if(candidates.Count == 0) return;
			int selected = candidates[random.Next(candidates.Count)];
			int id;
			EdgeGene gene = genotype.genes[selected];
			if(nodeGenerations.ContainsKey(gene.innovation)) id = nodeGenerations[gene.innovation];
			else {
				id = nodeIds++;
				nodeGenerations[gene.innovation] = id;
			}
			genotype.nodes.Add(id);
			genotype.genes[selected].enabled = false;
			genotype.genes.Add(new EdgeGene(gene.inNode, id, 1, GetInnovation(gene.inNode, id)));
			genotype.genes.Add(new EdgeGene(id, gene.outNode, gene.weight, GetInnovation(id, gene.outNode)));
		}

		public void MutateEnable(Genotype genotype, Random random) {
			List<int> candidates = new List<int>();
			for(int i = 0; i < genotype.genes.Count; i++) {
				if(!genotype.genes[i].enabled) candidates.Add(i);
			}
			if(candidates.Count == 0) return;
			int selected = candidates[random.Next(candidates.Count)];
			genotype.genes[selected].enabled = true;
		}

		public void MutateDisable(Genotype genotype, Random random) {
			List<int> candidates = new List<int>();
			for(int i = 0; i < genotype.genes.Count; i++) {
				if(genotype.genes[i].enabled) candidates.Add(i);
			}
			if(candidates.Count == 0) return;
			int selected = candidates[random.Next(candidates.Count)];
			genotype.genes[selected].enabled = false;
		}

		public void MutateWeight(Genotype genotype, Random random) {
			if(genotype.genes.Count == 0) return;
			int selected = random.Next(genotype.genes.Count);
			if(random.NextDouble() < shiftChance) {
				genotype.genes[selected].weight += ((float)random.NextDouble())*shiftStep*2 - shiftStep;
			}
			else genotype.genes[selected].weight = ((float)random.NextDouble())*4f - 2f;
		}

	}

}