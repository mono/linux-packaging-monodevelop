#!/bin/sh

find . \( -name "*.am" -or -name "*.in" \) -exec perl -pe 's!-r\:\$\(top_(src|build)dir\)/(contrib|build/bin)/Mono\.Addins\.(Gui\.|Setup\.)?dll!-r:/usr/lib/cli/mono-addins-0.2/Mono.Addins.${3}dll!g' -i {} \;

find . \( -name "*.am" -or -name "*.in" \) -exec perl -pe 's!-r:\$\(top_(src|build)dir\)/(contrib|build/bin)/Mono.Cecil.dll!\$(shell pkg-config --libs mono-cecil)!g' -i {} \;

find . \( -name "*.am" -or -name "*.in" \) -exec perl -pe 's!-r:\$\(top_(src|build)dir\)/(contrib|build/bin)/log4net.dll!\$(shell pkg-config --libs log4net)!g' -i {} \;
