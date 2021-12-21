# kolon

Kolon is an easy-to-use programming language built with c#

---

# Prerequisites

**IMPORTANT** requires [.NET 6.0](https://dotnet.microsoft.com/en-us/download)

---

# Usage
```
git clone https://github.com/denniskaydalov/Kolon.git
```
To run your Kolon code, start by navigating to:
```
.\Kolon\KolonApplication
```
Then run:
```
dotnet run .\Program.cs .\Kolon.txt
```
This executes all the code from the `Kolon.txt` file.

To run your own code, replace `.\Kolon.txt` with the path of your own text file, and hit enter.

---

# Examples

## Declaring variables

The structure of a variable is `type name`, or `type name = value`.

```
int a

int b = 1

bool c = false
```

## Comments
A comment in Kolon begins with `//`, all text after the comment is ignored.
```
// this is a comment

int d = 4 // this is also a comment
```

## Referencing variables

To reference a variable, add a `$` before the variable name: `$name`.

```
$a = 4

$b = $a
```

## Operators
Operators in Kolon include: `+`, `-`, `/`, `*`, and `%`.
```
$a = 1 + 2

$b = 2 % 0 + $a
```
You can also include parentheses:
```
$a = (1 + 2) * 3
```
Using `-` before a set of parentheses will result in the expression in the parentheses being negative.
```
$a = -(1) //a is now equal to -1
```
You can increment and decrement using `++` and `--`, respectively.
```
$a++

$b--
```
You can use the shorthand `+=` to add a value to a variable, and `-=` to substract a value from a variable. 
```
$a += 1

$b -= $a
```
## Declaring methods
The structure of a method is `func name () {`, all code until `}` will be part of the method.
```
func foo () {
    int i = 1
}
```

## Calling methods
To call a method, write the name of the method, followed by its arguments: `name argument1, argument2...`
```
foo
```
`print` is a custom method which prints the value of its arguments to the console, it accepts one argument.
```
print 1 // prints 1 to the console

print $a // prints the value of a to the console
```






