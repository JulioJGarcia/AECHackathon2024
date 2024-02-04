import React from 'react';
import { IDS, Specification, Requirement } from '../interfaces/IDSInterface'; // Replace with the actual path to your IDS interface

interface Props {
  parsedData: IDS | null;
}

const IDSViewer: React.FC<Props> = ({ parsedData }) => {
  if (!parsedData) {
    return null;
  }

  const renderSpecifications = (specification: Specification[]|Specification) => {
    if (!specification) {
        return null;
      }
    if(specification as Specification){
        const spec = (specification as Specification).specification;
         return (
            <div className="specification">
              <h3>{`Specification $index`}</h3>
              <p><strong>Name:</strong> {spec.name}</p>
              <p><strong>IFC Version:</strong> {spec.ifcVersion}</p>
              <p><strong>Description:</strong> {spec.description}</p>
              <p><strong>Applicability:</strong> {spec.applicability.entity.name.simpleValue}</p>
              {renderRequirements(spec.requirements.property)}
            </div>
          );
    }
    const spec = specification as Specification[];
    return spec.map((specification, index) => {
        const spec = (specification as Specification).specification;
    return (
      <div className="specification">
        <h3>{`Specification $index`}</h3>
        <p><strong>Name:</strong> {spec.name}</p>
        <p><strong>IFC Version:</strong> {spec.ifcVersion}</p>
        <p><strong>Description:</strong> {spec.description}</p>
        <p><strong>Applicability:</strong> {spec.applicability.entity.name.simpleValue}</p>
        {renderRequirements(spec.requirements.property)}
      </div>
    )});
};

const getDatatypeValue = (datatype: string | null| undefined): string => {
  if (typeof datatype === 'string') {
    return datatype;
  } else {
    return "";
  } 
};
const getMinOccursValue = (minOccurs: string | null| undefined): string => {
  if (typeof minOccurs === 'string') {
    return minOccurs;
  } else {
    return "";
  } 
};

const getMaxOccursValue = (maxOccurs: string | null| undefined): string => {
  if (typeof maxOccurs === 'string') {
    return maxOccurs;
  } else {
    return "";
  } 
};
  const renderRequirements = (requirements: Requirement[]) => {
    // {renderValue(requirement..value)}
    return requirements.map((requirement, index) => (
      <div key={index} className="requirement">
        <p><strong>Datatype:</strong> {getDatatypeValue(requirement.$?.datatype)}</p>
        <p><strong>Min Occurs:</strong> {getMinOccursValue(requirement.$?.minOccurs)}</p>
        <p><strong>Max Occurs:</strong> {getMaxOccursValue(requirement.$?.maxOccurs)}</p>
        <p><strong>Property Set Base:</strong> {getPropertySetBase(requirement.propertySet)}</p>
      <p><strong>Property Set Pattern:</strong> {getPropertySetPattern(requirement.propertySet)}</p>
        <p><strong>Property Name:</strong> {requirement.name?.simpleValue}</p>
        <p><strong>Property Value:</strong> {getPropertyValue(requirement)}</p>
      </div>
    ));
  };

  const getPropertyValue = (requirement: Requirement): string | undefined => {
    // Check if 'xs:enumeration' exists in 'value.xs:restriction'
    const enumeration = requirement.value?.["xs:restriction"]?.["xs:enumeration"];
  
    // Join the values into a comma-separated string
    return enumeration?.map(enumVal => enumVal.$?.value).join(', ');
  };
  const getPropertySetBase = (propertySet: Requirement['propertySet']) => {
    return propertySet["xs:restriction"]?.$?.base || "";
  };
  
  const getPropertySetPattern = (propertySet: Requirement['propertySet']) => {
    const xsRestriction = propertySet["xs:restriction"];
    
    if (Array.isArray(xsRestriction)) {
      // If it's an array, you may need to handle multiple items.
      // For now, just pick the first item and proceed.
      const firstItem = xsRestriction[0];
      return firstItem ? (firstItem["xs:pattern"]?.["$?.value"] || "") : "";
    } else {
      // If it's not an array, proceed with the usual logic
      return xsRestriction?.["xs:pattern"]?.$?.value|| "";
    }
  };
  const renderValue = (value?: Requirement[][]) => {
    if (!value) {
      return null;
    }
{/* <div className="value">
        <p><strong>Value Base:</strong> {value.restriction.base}</p>
        <p><strong>Value Enumeration:</strong> {value.restriction.enumeration.join(', ')}</p>
      </div> */}
    return (
      <div className="value">

      </div>
    );
  };
//  
  return (
    <div className="ids-viewer">
      <h2>IDS Viewer</h2>
      <div className="info">
        <h3>Info</h3>
        <p><strong>Title:</strong> {parsedData.ids?.info?.title}</p>
        <p><strong>Copyright:</strong> {parsedData.ids?.info?.copyright}</p>
        <p><strong>Version:</strong> {parsedData.ids?.info?.version}</p>
        <p><strong>Description:</strong> {parsedData.ids?.info?.description}</p>
        <p><strong>Milestone:</strong> {parsedData.ids?.info?.milestone}</p>
      </div>
      <div className="specifications">
        <h3>Specifications</h3>
        {renderSpecifications(parsedData.ids?.specifications)}
      </div>
    </div>
  );
};

export default IDSViewer;