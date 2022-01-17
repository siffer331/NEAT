using System;
using System.Collections.Generic;
using Godot;

namespace NEAT {

	public class Node{
		public double value, oldValue;
		public List<EdgeGene> incommingEdges = new List<EdgeGene>();

		public Node() {
			value = 0;
			oldValue = 0;
		}
	}

	public class Phenotype {

		public List<Node> nodes = new List<Node>();
		public Dictionary<int, int> indexes = new Dictionary<int, int>();
		public int inputs, outputs;
		public float fitness = 0f;

		public Phenotype(int inputs, int outputs, Genotype genotype) {
			this.inputs = inputs;
			this.outputs = outputs;
			foreach(int node in genotype.nodes) {
				indexes[node] = nodes.Count;
				nodes.Add(new Node());
			}
			foreach(EdgeGene edge in genotype.genes) {
				nodes[indexes[edge.outNode]].incommingEdges.Add(edge);
			}
		}

		public double[] Propagate(double[] data) {
			if(data.Length != inputs) throw new Exception("Data length does not match inputs");
			bool[] visited = new bool[nodes.Count];
			for(int i = 0; i < nodes.Count; i++) {
				nodes[i].oldValue = nodes[i].value;
				visited[i] = false;
				if(i < inputs) nodes[i].value = data[i];
				else nodes[i].value = 0;
			}
			double Run(int index) {
				if(visited[index]) return nodes[index].value;
				visited[index] = true;
				double sum = 0;
				foreach(EdgeGene edge in nodes[index].incommingEdges) {
					if(edge.enabled) sum += Run(indexes[edge.inNode])*edge.weight;
				}
				nodes[index].value = Sigmoid(sum);
				return nodes[index].value;
			}
			double[] output = new double[outputs];
			for(int i = inputs; i < inputs+outputs; i++) output[i-inputs] = Run(i);
			return output;
		}

		public double Sigmoid(double value) {
			return 1/(1+Math.Exp(-value));
		}

	}

}