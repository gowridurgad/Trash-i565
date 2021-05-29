# Trdelete

Reads a parse tree from stdin, deletes nodes in the tree using
the specified XPath expression, and writes the modified tree
to stdout. The input and output are Parse Tree Data.

# Usage

    trdelete <string>

# Example

    trparse -f Java.g4 | trdelete " //parserRuleSpec[RULE_REF/text() = 'normalAnnotation']" | trtree | vim -

# Current version

0.8.1 -- Improved help and documentation.