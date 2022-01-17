using System;
using System.Collections.Generic;


namespace NEAT {

	public class EdgeGene {
		public int inNode, outNode;
		public double weight;
		public bool enabled;
		public int innovation;

		public EdgeGene(int inNode, int outNode, double weight, int innovation, bool enabled = true) {
			this.inNode = inNode;
			this.outNode = outNode;
			this.weight = weight;
			this.innovation = innovation;
			this.enabled = enabled;
		}

		public EdgeGene Clone() {
			return new EdgeGene(inNode, outNode, weight, innovation, enabled);
		}
	}

	public class Genotype {

		public List<EdgeGene> genes = new List<EdgeGene>();
		public List<int> nodes = new List<int>();
		public float fitness;
		public float adjustedFitness;

		public Genotype(int inputs, int outputs) {
			for(int i = 0; i < inputs+outputs; i++) nodes.Add(i);
		}

		public Genotype Clone() {
			Genotype result = new Genotype(0,0);
			foreach(EdgeGene gene in genes) result.genes.Add(gene.Clone());
			foreach(int node in nodes) result.nodes.Add(node);
			result.fitness = fitness;
			result.adjustedFitness = adjustedFitness;
			return result;
		}

	}


}