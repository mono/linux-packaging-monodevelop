#!/bin/sh

echo "Updating nunit refereces"
find . \( -name "*.am" -or -name "*.in" \) -exec perl -pe 's!-r:\$\(top_srcdir\)/src/addins/NUnit/lib/nunit.core.dll!\$(shell pkg-config --libs nunit)!g' -i {} \;
find . \( -name "*.am" -or -name "*.in" \) -exec perl -pe 's!-r:\$\(top_srcdir\)/src/addins/NUnit/lib/nunit.framework.dll!\$(shell pkg-config --libs nunit)!g' -i {} \;
