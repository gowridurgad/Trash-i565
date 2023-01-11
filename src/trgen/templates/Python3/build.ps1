# Template generated code from trgen <version>
<tool_grammar_tuples:{x |
$(& java -jar <antlr_tool_path> <x.GrammarFileName> -encoding <antlr_encoding> -Dlanguage=Python3 <x.AntlrArgs> <antlr_tool_args:{y | <y> } > ; $compile_exit_code = $LASTEXITCODE) | Write-Host
if($compile_exit_code -ne 0){
    exit $compile_exit_code
\}
}>

$(& pip install -r requirements.txt ; $compile_exit_code = $LASTEXITCODE ) | Write-Host
exit $compile_exit_code