using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Linq;

namespace Structures
{
    public class BTree<K, P> where K : IComparable<K>
    {

        public Node<K, P> Root { get; private set; }

        public int Degree { get; private set; }

        public int Height { get; private set; }


        public BTree(int degree)
        {
            if (degree < 2)
            {
                throw new ArgumentException("BTree degree must be at least 2", "degree");
            }

            this.Root = new Node<K, P>(degree);
            this.Degree = degree;
            this.Height = 1;
        }

        public P Search(K key)
        {
            return this.SearchInternal(this.Root, key).Pointer;
        }

        public bool IsEmpty()
        {

            if (Root == null)
                return true;
            else
                return false;

        }

        public void WipeOut()
        {

            Root = null;

        }

        public void Insert(K newKey, P newPointer)
        {
            // there is space in the root node
            if (!this.Root.HasReachedMaxItems)
            {
                this.InsertNonFull(this.Root, newKey, newPointer);
                return;
            }

            // need to create new node and have it split
            Node<K, P> oldRoot = this.Root;
            this.Root = new Node<K, P>(this.Degree);
            this.Root.Children.Add(oldRoot);
            this.SplitChild(this.Root, 0, oldRoot);
            this.InsertNonFull(this.Root, newKey, newPointer);

            this.Height++;
        }

        public void Delete(K keyToDelete)
        {
            this.DeleteInternal(this.Root, keyToDelete);

            // if root's last entry was moved to a child node, remove it
            if (this.Root.Items.Count == 0 && !this.Root.IsLeaf)
            {
                this.Root = this.Root.Children.Single();
                this.Height--;
            }
        }

        private void DeleteInternal(Node<K, P> node, K keyToDelete)
        {
            int i = node.Items.TakeWhile(entry => keyToDelete.CompareTo(entry.Key) > 0).Count();

            // found key in node, so delete if from it
            if (i < node.Items.Count && node.Items[i].Key.CompareTo(keyToDelete) == 0)
            {
                this.DeleteKeyFromNode(node, keyToDelete, i);
                return;
            }

            // delete key from subtree
            if (!node.IsLeaf)
            {
                this.DeleteKeyFromSubtree(node, keyToDelete, i);
            }
        }

        private void DeleteKeyFromSubtree(Node<K, P> parentNode, K keyToDelete, int subtreeIndexInNode)
        {
            Node<K, P> childNode = parentNode.Children[subtreeIndexInNode];

            // node has reached min # of Items, and removing any from it will break the btree property,
            // so this block makes sure that the "child" has at least "degree" # of nodes by moving an 
            // entry from a sibling node or merging nodes
            if (childNode.HasReachedMinItems)
            {
                int leftIndex = subtreeIndexInNode - 1;
                Node<K, P> leftSibling = subtreeIndexInNode > 0 ? parentNode.Children[leftIndex] : null;

                int rightIndex = subtreeIndexInNode + 1;
                Node<K, P> rightSibling = subtreeIndexInNode < parentNode.Children.Count - 1
                                                ? parentNode.Children[rightIndex]
                                                : null;

                if (leftSibling != null && leftSibling.Items.Count > this.Degree - 1)
                {
                    // left sibling has a node to spare, so this moves one node from left sibling 
                    // into parent's node and one node from parent into this current node ("child")
                    childNode.Items.Insert(0, parentNode.Items[subtreeIndexInNode]);
                    parentNode.Items[subtreeIndexInNode] = leftSibling.Items.Last();
                    leftSibling.Items.RemoveAt(leftSibling.Items.Count - 1);

                    if (!leftSibling.IsLeaf)
                    {
                        childNode.Children.Insert(0, leftSibling.Children.Last());
                        leftSibling.Children.RemoveAt(leftSibling.Children.Count - 1);
                    }
                }
                else if (rightSibling != null && rightSibling.Items.Count > this.Degree - 1)
                {
                    // right sibling has a node to spare, so this moves one node from right sibling 
                    // into parent's node and one node from parent into this current node ("child")
                    childNode.Items.Add(parentNode.Items[subtreeIndexInNode]);
                    parentNode.Items[subtreeIndexInNode] = rightSibling.Items.First();
                    rightSibling.Items.RemoveAt(0);

                    if (!rightSibling.IsLeaf)
                    {
                        childNode.Children.Add(rightSibling.Children.First());
                        rightSibling.Children.RemoveAt(0);
                    }
                }
                else
                {
                    // this block merges either left or right sibling into the current node "child"
                    if (leftSibling != null)
                    {
                        childNode.Items.Insert(0, parentNode.Items[subtreeIndexInNode]);
                        var oldItems = childNode.Items;
                        childNode.Items = leftSibling.Items;
                        childNode.Items.AddRange(oldItems);
                        if (!leftSibling.IsLeaf)
                        {
                            var oldChildren = childNode.Children;
                            childNode.Children = leftSibling.Children;
                            childNode.Children.AddRange(oldChildren);
                        }

                        parentNode.Children.RemoveAt(leftIndex);
                        parentNode.Items.RemoveAt(subtreeIndexInNode);
                    }
                    else
                    {
                        Debug.Assert(rightSibling != null, "Node should have at least one sibling");
                        childNode.Items.Add(parentNode.Items[subtreeIndexInNode]);
                        childNode.Items.AddRange(rightSibling.Items);
                        if (!rightSibling.IsLeaf)
                        {
                            childNode.Children.AddRange(rightSibling.Children);
                        }

                        parentNode.Children.RemoveAt(rightIndex);
                        parentNode.Items.RemoveAt(subtreeIndexInNode);
                    }
                }
            }

            // at this point, we know that "child" has at least "degree" nodes, so we can
            // move on - this guarantees that if any node needs to be removed from it to
            // guarantee BTree's property, we will be fine with that
            this.DeleteInternal(childNode, keyToDelete);
        }

        private void DeleteKeyFromNode(Node<K, P> node, K keyToDelete, int keyIndexInNode)
        {
            // if leaf, just remove it from the list of Items (we're guaranteed to have
            // at least "degree" # of Items, to BTree property is maintained
            if (node.IsLeaf)
            {
                node.Items.RemoveAt(keyIndexInNode);
                return;
            }

            Node<K, P> predecessorChild = node.Children[keyIndexInNode];
            if (predecessorChild.Items.Count >= this.Degree)
            {
                Item<K, P> predecessor = this.DeletePredecessor(predecessorChild);
                node.Items[keyIndexInNode] = predecessor;
            }
            else
            {
                Node<K, P> successorChild = node.Children[keyIndexInNode + 1];
                if (successorChild.Items.Count >= this.Degree)
                {
                    Item<K, P> successor = this.DeleteSuccessor(predecessorChild);
                    node.Items[keyIndexInNode] = successor;
                }
                else
                {
                    predecessorChild.Items.Add(node.Items[keyIndexInNode]);
                    predecessorChild.Items.AddRange(successorChild.Items);
                    predecessorChild.Children.AddRange(successorChild.Children);

                    node.Items.RemoveAt(keyIndexInNode);
                    node.Children.RemoveAt(keyIndexInNode + 1);

                    this.DeleteInternal(predecessorChild, keyToDelete);
                }
            }
        }

        private Item<K, P> DeletePredecessor(Node<K, P> node)
        {
            if (node.IsLeaf)
            {
                var result = node.Items[node.Items.Count - 1];
                node.Items.RemoveAt(node.Items.Count - 1);
                return result;
            }

            return this.DeletePredecessor(node.Children.Last());
        }

        private Item<K, P> DeleteSuccessor(Node<K, P> node)
        {
            if (node.IsLeaf)
            {
                var result = node.Items[0];
                node.Items.RemoveAt(0);
                return result;
            }

            return this.DeletePredecessor(node.Children.First());
        }
              
        private Item<K, P> SearchInternal(Node<K, P> node, K key)
        {
            int i = node.Items.TakeWhile(entry => key.CompareTo(entry.Key) > 0).Count();

            if (i < node.Items.Count && node.Items[i].Key.CompareTo(key) == 0)
            {
                return node.Items[i];
            }

            return node.IsLeaf ? null : this.SearchInternal(node.Children[i], key);
        }

        private void SplitChild(Node<K, P> parentNode, int nodeToBeSplitIndex, Node<K, P> nodeToBeSplit)
        {
            var newNode = new Node<K, P>(this.Degree);

            parentNode.Items.Insert(nodeToBeSplitIndex, nodeToBeSplit.Items[this.Degree - 1]);
            parentNode.Children.Insert(nodeToBeSplitIndex + 1, newNode);

            newNode.Items.AddRange(nodeToBeSplit.Items.GetRange(this.Degree, this.Degree - 1));

            // remove also Items[this.Degree - 1], which is the one to move up to the parent
            nodeToBeSplit.Items.RemoveRange(this.Degree - 1, this.Degree);

            if (!nodeToBeSplit.IsLeaf)
            {
                newNode.Children.AddRange(nodeToBeSplit.Children.GetRange(this.Degree, this.Degree));
                nodeToBeSplit.Children.RemoveRange(this.Degree, this.Degree);
            }
        }

        private void InsertNonFull(Node<K, P> node, K newKey, P newPointer)
        {
            int positionToInsert = node.Items.TakeWhile(entry => newKey.CompareTo(entry.Key) >= 0).Count();

            // leaf node
            if (node.IsLeaf)
            {
                node.Items.Insert(positionToInsert, new Item<K, P>() { Key = newKey, Pointer = newPointer });
                return;
            }

            // non-leaf
            Node<K, P> child = node.Children[positionToInsert];
            if (child.HasReachedMaxItems)
            {
                this.SplitChild(node, positionToInsert, child);
                if (newKey.CompareTo(node.Items[positionToInsert].Key) > 0)
                {
                    positionToInsert++;
                }
            }

            this.InsertNonFull(node.Children[positionToInsert], newKey, newPointer);
        }
    }
}
