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
      requirements: Requirement[];
  }}
  
  interface Requirement {
    property: {
      datatype: string;
      minOccurs: string;
      maxOccurs: string;
      propertySet: {
        restriction: {
          base: string;
          pattern: string;
        };
      };
      name: {
        simpleValue: string;
      };
      value?: {
        restriction: {
          base: string;
          enumeration: string[];
        };
      };
    };
  }
  
  export type { IDS,IdsInfo, Specification, Requirement };