import React from 'react';
import { Image, Text, StyleSheet } from 'react-native';
import { Card, Paragraph, TouchableRipple } from 'react-native-paper';

export const GMapsPredictionCard = ({ location, onPress, onLongPress }) => (
  <Card style={styles.searchResult} onPress={onPress} onLongPress={onLongPress}>
    <Card.Title title={location.structured_formatting.main_text} />
    <Card.Content>
      <Paragraph>
        { location.description}
      </Paragraph>
    </Card.Content>
  </Card>
);

export const GMapsCard = ({
  location, onPress, onLongPress, highlight
}) => (
  <Card style={styles.searchResult} onPress={onPress} onLongPress={onLongPress} elevation={highlight ? 5 : 1}>
    <Card.Title title={location.details.name} />
    <Card.Content>
      <Paragraph>
        {location.details.formatted_address}
      </Paragraph>
      <Image style={{ width: 20, height: 20 }} source={{ uri: location.details.icon }} />
    </Card.Content>
  </Card>
);

export const CustomLocationCard = ({
  location, onPress, onLongPress, highlight
}) => (
  <Card style={styles.searchResult} onPress={onPress} onLongPress={onLongPress} elevation={highlight ? 5 : 1}>
    <Card.Title title={location.name} />
    <Card.Content>
      <Paragraph>
        {location.address}
      </Paragraph>
    </Card.Content>
  </Card>
);

const LocationCard = ({
  location, onPressPrediction, onPressGMaps, onPressCustom, onLongPress, highlight
}) => {
  const { kind } = location;
  const [Component, onPress] = (kind === 'google_maps_locations_predictions'
    ? [GMapsPredictionCard, onPressPrediction]
    : kind === 'google_maps_locations'
      ? [GMapsCard, onPressGMaps] : [CustomLocationCard, onPressCustom]);
  return <Component location={location} onPress={onPress} onLongPress={onLongPress} highlight={highlight} />;
};

const AddKindToLocation = (location) => {

};

const styles = StyleSheet.create({
  container: {
    height: '100%',
    width: '100%'
  },
  searchbar: {
    margin: 5
  },
  searchResult: {
    margin: 5
  },
  selectedUserContainer: {
    margin: 5
  },
  fab: {
    margin: 10, right: 0, position: 'absolute', bottom: 0
  }
});

export default LocationCard;
