import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
import React, { useContext, useEffect, useState } from 'react';
import {
  Image, StyleSheet, View
} from 'react-native';
import ImagePicker from 'react-native-image-crop-picker';
import {
  ActivityIndicator, Button, Card
} from 'react-native-paper';
import { getImagesInfo, uploadImage } from '../../Requests';
import { store } from '../../Store';

const DebtImage = ({ id }) => {
  const { state } = useContext(store);
  const [imageData, setImageData] = useState(null);
  const [imageSize, setImageSize] = useState({ width: 0, height: 0 });

  useEffect(() => {
    getImagesInfo({ state }, id).then((info) => {
      Image.getSize(info[0].image_url, (width, height) => {
        setImageSize({ width, height });
        setImageData(info[0]);
      }).catch(console.log);
    });
  }, [id, state]);

  return (
    imageData
      ? (
        <Card style={{ margin: 5 }}>
          <Card.Content>
            <Image
              style={{ width: '100%', height: undefined, aspectRatio: imageSize.width / imageSize.height || 0 }}
              source={{ uri: imageData.image_url }}
            />
          </Card.Content>
        </Card>
      )
      : <ActivityIndicator />
  );
};

const Images = ({ images }) => (images ? images.map((i) => <DebtImage key={i} id={i} />) : <></>);

const Debts = ({ images, onAdd }) => {
  const { state } = useContext(store);
  return (
    <Card style={styles.section} elevation={0}>
      <Card.Title title="Debts" />
      <Card.Content>
        <Images images={images} />
        <View style={{ flexDirection: 'row', justifyContent: 'space-evenly' }}>
          <Button
            style={styles.addButton}
            onPress={onAdd
            }
          >
            <Icon name="cash-usd" size={25} />
          </Button>
          <Button
            style={styles.addButton}
            onPress={
          () => {
            ImagePicker.openPicker({
              cropping: true
            }).then((i) => uploadImage({ state }, state.event.id, i));
          }
        }
          >
            <Icon name="image-plus" size={25} />
          </Button>
        </View>
      </Card.Content>
    </Card>
  );
};

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

export default Debts;
