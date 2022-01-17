using System;


namespace NEAT {

	public class Crossover {

		public float excessConstant = 1f;
		public float disjointConstant = 1f;
		public float weightConstant = 0.4f;
		public float differenceThreshold = 3f;
		public float disableChance = 0.2f;
		public float crossoverChance = 0.75f;

		public Crossover() {
		}

		public Genotype MakeOffspring(Genotype a, Genotype b, Random random) {
			Genotype child = new Genotype(0,0);
			int indexA = 0;
			int indexB = 0;
			while(indexA < a.genes.Count && indexB < b.genes.Count) {
				if(indexA == a.genes.Count) {
					indexB++;
				} else if(indexB == b.genes.Count) {
					child.genes.Add(a.genes[indexA].Clone());
					indexA++;
				} else if(a.genes[indexA].innovation < b.genes[indexB].innovation) {
					child.genes.Add(a.genes[indexA].Clone());
					indexA++;
				} else if(b.genes[indexB].innovation < a.genes[indexA].innovation) {
					indexB++;
				} else {
					if(random.Next(2) == 0) child.genes.Add(a.genes[indexA].Clone());
					else child.genes.Add(b.genes[indexB].Clone());
					if((!a.genes[indexA].enabled || !b.genes[indexB].enabled) && (float)random.NextDouble() <= disableChance) child.genes[child.genes.Count-1].enabled = false;
					indexA++;
					indexB++;
				}
			}
			for(int i = 0; i < a.nodes.Count; i++) child.nodes.Add(a.nodes[i]);
			return child;
		}

		public bool Alike(Genotype a, Genotype b) {
			return Distance(a, b) <= differenceThreshold;
		}

		public float Distance(Genotype a, Genotype b) {
			float excess = 0f;
			float disjoint = 0f;
			float weightDifference = 0f;
			float matches = 0f;
			
			int indexA = 0;
			int indexB = 0;

			while(indexA < a.genes.Count && indexB < b.genes.Count) {
				if(indexA == a.genes.Count) {
					excess++;
					indexB++;
				} else if(indexB == b.genes.Count) {
					excess++;
					indexA++;
				} else if(a.genes[indexA].innovation < b.genes[indexB].innovation) {
					disjoint++;
					indexA++;
				} else if(b.genes[indexB].innovation < a.genes[indexA].innovation) {
					disjoint++;
					indexB++;
				} else {
					weightDifference += (float)Math.Abs(a.genes[indexA].weight - b.genes[indexB].weight);
					matches++;
					indexA++;
					indexB++;
				}
			}
			float n = (float)Math.Max(a.genes.Count, b.genes.Count);
			return excess*excessConstant/n + disjoint*disjointConstant/n + weightDifference*weightConstant/matches;
		}
	}

}