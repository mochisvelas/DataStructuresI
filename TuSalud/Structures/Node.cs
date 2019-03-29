using System;
using System.Collections.Generic;
using System.Text;

namespace Structures
{
    public class Node<K, P>
    {
        private int degree;

        public Node(int degree)
        {
            this.degree = degree;
            this.Children = new List<Node<K, P>>(degree);
            this.Items = new List<Item<K, P>>(degree);
        }

        public List<Node<K, P>> Children { get; set; }

        public List<Item<K, P>> Items { get; set; }

        public bool IsLeaf
        {
            get
            {
                return this.Children.Count == 0;
            }
        }

        public bool HasReachedMaxItems
        {
            get
            {
                return this.Items.Count == (2 * this.degree) - 1;
            }
        }

        public bool HasReachedMinItems
        {
            get
            {
                return this.Items.Count == this.degree - 1;
            }
        }
    }
}
