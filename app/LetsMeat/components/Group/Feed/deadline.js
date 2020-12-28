import React from 'react';
import { StyleSheet } from 'react-native';
import { Card } from 'react-native-paper';
import { TimeCard } from './times';

const Deadline = ({ time }) => (
  <Card elevation={0} style={styles.section}>
    <Card.Title title="Deadline" />
    <TimeCard time={new Date(time)} />
  </Card>
);

const styles = StyleSheet.create({
  eventTitle: {
    fontSize: 30,
    marginHorizontal: 20,
    marginTop: 20
  },
  container: {
    width: '100%',
    height: '100%'
  },
  section: {
    margin: 10
  },
  card: {
    margin: 25
  },
  addButton: {
    marginBottom: 10
  },
  timeCard: {
    margin: 5,
    justifyContent: 'center'
  }
});

export default Deadline;
