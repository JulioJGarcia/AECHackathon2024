interface IDS {
  ids:{
    info: IdsInfo;
    specifications: Specification[];
  }

  }
  interface IdsInfo{
    title: string;
      copyright: string;
      version: number;
      description: string;
      milestone: string;
  }
  interface Specification {
  specification:{
      name: string;
      ifcVersion: string;
      description: string;
      applicability: {
        entity: {
          name: {
            simpleValue: string;
          };
        };
      };
      requirements: {
        property: Requirement[];
      };
  }}
  
  interface Requirement {
    $?:{
      datatype: string;
      minOccurs: string;
      maxOccurs: string;
    }
      propertySet: {
   
          "xs:restriction"?: {
            $?: {
              base: string;
            };
          "xs:pattern"?: {
            $?: {
              value: string;
            };
          };
  
      };
    };
      name: {
        simpleValue: string;
      };
      value?: {
        "xs:restriction"?: {
          base: string;
          'xs:enumeration'?: enumValue[];
        };
      };
  };
  interface enumValue{
    $?: {
      value: string;
    };
  };
  
  export type { IDS,IdsInfo, Specification, Requirement };