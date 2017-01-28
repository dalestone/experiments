var nodes = [ 
    { "id": "rootnode/subnode/node" },
    { "id": "level1/level2/level3/level4" },
    { "id": "1/2/3/4/5/6/7/8/9/10"}
];

var input = nodes.map(function(node) { return node.id });
var ouput = [];

/**
* Builds an array of parent/child nodes
* @param {Array} input
* @param {Array} ouput
*/
function createTree(input, output) {
    for (var i = 0; i < input.length; i++) {
        var chain = input[i].split('/');
        var currentNode = output;
        for (var j = 0; j < chain.length; j++) {
            var wantedNode = chain[j];
            var lastNode = currentNode;
            for (var k = 0; k < currentNode.length; k++) {
                if (currentNode[k].name == wantedNode) {
                    currentNode = currentNode[k].children;
                    break;
                }
            }
            // If we couldn't find an item in this list of children
            // that has the right name, create one:
            if (lastNode == currentNode) {
                var newNode = currentNode[k] = { name: wantedNode, children: [] };
                currentNode = newNode.children;
            }
        }

    }
}

        
/** 
* Build a tree view
* @param{Array} input
* @paremt{Element} container
*/
function renderTree(input, container) {
    var tree = document.createElement('ul');
    var rootNode;
    var childNodes;

    function renderChildNodes(rootNode, children) {
        var childRootNode = document.createElement('ul');
        var childNode;

        for (var j = 0; j < children.length; j++) {
            childNode = document.createElement('li');
            childNode.innerHTML = children[j].name;
            childRootNode.appendChild(childNode);

            if (children[j].children) {
                renderChildNodes(childNode, children[j].children);
            }
        }
        rootNode.appendChild(childRootNode);

        return childRootNode;
    }

    for (var i = 0; i < input.length; i++) {
        rootNode = document.createElement('li');
        childNodes = input[i].children;

        rootNode.innerHTML = input[i].name;
        tree.appendChild(rootNode);
        
        rootNode.appendChild(renderChildNodes(rootNode, childNodes));
    }               

    if (arguments.length === 2) {
        container.appendChild(tree);
    } 

    return tree;
}