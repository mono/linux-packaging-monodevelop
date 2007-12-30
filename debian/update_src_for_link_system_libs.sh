#!/bin/sh

#echo "Updating Mono.Addins references"
##find . \( -name "*.am" -or -name "*.in" \) -exec perl -pe 's!-r\:\$\(top_(src|build)dir\)/(contrib|build/bin)/Mono\.Addins\.(Gui\.|Setup\.)?dll!-r:/usr/lib/cli/mono-addins-0.2/Mono.Addins.${3}dll!g' -i {} \;
#find . \( -name "*.am" -or -name "*.in" \) -exec perl -pe 's!-r\:\$\(top_(src|build)dir\)/(contrib|build/bin)/Mono\.Addins\.dll!\$(shell pkg-config --libs mono-addins)!g' -i {} \;
#find . \( -name "*.am" -or -name "*.in" \) -exec perl -pe 's!-r\:\$\(top_(src|build)dir\)/(contrib|build/bin)/Mono\.Addins\.Gui\.dll!\$(shell pkg-config --libs mono-addins-gui)!g' -i {} \;
#find . \( -name "*.am" -or -name "*.in" \) -exec perl -pe 's!-r\:\$\(top_(src|build)dir\)/(contrib|build/bin)/Mono\.Addins\.Setup\.dll!\$(shell pkg-config --libs mono-addins-setup)!g' -i {} \;

echo "Updating Mono.Cecil references"
find . \( -name "*.am" -or -name "*.in" \) -exec perl -pe 's!-r:\$\(top_(src|build)dir\)/(contrib|build/bin)/Mono.Cecil.dll!\$(shell pkg-config --libs mono-cecil)!g' -i {} \;
find . \( -name "*.am" -or -name "*.in" \) -exec perl -pe 's!-r:\$\(top_(src|build)dir\)/(contrib|build/bin)/Mono.Cecil.Mdb.dll!!g' -i {} \;

#echo "Updating log4net references"
#find . \( -name "*.am" -or -name "*.in" \) -exec perl -pe 's!-r:\$\(top_(src|build)dir\)/(contrib|build/bin)/log4net.dll!\$(shell pkg-config --libs log4net)!g' -i {} \;

echo "Deleting reject files (*.rej)"
find -name "*.rej" -delete
