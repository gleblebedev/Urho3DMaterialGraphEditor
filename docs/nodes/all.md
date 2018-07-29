# all


all returns true if all elements of x are true and false otherwise.

It is functionally equivalent to:

```
    bool all(bvec x)       // bvec can be bvec2, bvec3 or bvec4
    {
        bool result = true;
        int i;
        for (i = 0; i < x.length(); ++i)
        {
            result &= x[i];
        }
        return result;
    }
```

## all(bvec2)

### Parameter

x
  Type: bvec2

### Return Value

  Type: bool

<details><summary>JSON</summary>

```
{
  "Type": "all(bvec2)",
  "Name": "all(bvec2)",
  "Category": 1,
  "InputPins": [
    {
      "Connection": null,
      "Id": "x",
      "Type": "bvec2"
    }
  ],
  "OutputPins": [
    {
      "Id": "",
      "Type": "bool"
    }
  ]
}
```

</details>

## all(bvec3)

### Parameter

x
  Type: bvec3

### Return Value

  Type: bool

<details><summary>JSON</summary>

```
{
  "Type": "all(bvec3)",
  "Name": "all(bvec3)",
  "Category": 1,
  "InputPins": [
    {
      "Connection": null,
      "Id": "x",
      "Type": "bvec3"
    }
  ],
  "OutputPins": [
    {
      "Id": "",
      "Type": "bool"
    }
  ]
}
```

</details>

## all(bvec4)

### Parameter

x
  Type: bvec4

### Return Value

  Type: bool

<details><summary>JSON</summary>

```
{
  "Type": "all(bvec4)",
  "Name": "all(bvec4)",
  "Category": 1,
  "InputPins": [
    {
      "Connection": null,
      "Id": "x",
      "Type": "bvec4"
    }
  ],
  "OutputPins": [
    {
      "Id": "",
      "Type": "bool"
    }
  ]
}
```

</details>

