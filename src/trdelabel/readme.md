# Delabel

Remove all labels from an Antlr4 grammar.

    "expr : lhs=expr (PLUS | MINUS) rhs=expr # foobar1 ....." => "expr : expr (PLUS | MINUS) expr ....."

# Usage

    trdelabel

# Example

    trparse A.g4 | delabel | trprint

# Current version

0.16.6 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.