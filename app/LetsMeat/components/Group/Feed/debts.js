import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
import React, { useContext, useEffect, useState } from 'react';
import {
  Image, StyleSheet, View
} from 'react-native';
import ImagePicker from 'react-native-image-crop-picker';
import {
  ActivityIndicator, Button, Card
} from 'react-native-paper';
import { getImagesInfo, uploadImage, deleteImage } from '../../Requests';
import { store } from '../../Store';

const DebtImage = ({ id }) => {
  const { state, dispatch } = useContext(store);
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
          <Card.Actions style={{ justifyContent: 'space-evenly' }}>
            <Button onPress={
              () => {
                deleteImage({ state }, id).then(() => {
                  dispatch({ type: 'REMOVE_IMAGE', imageId: id });
                });
              }
            }
            >
              <Icon name="delete" size={25} />
            </Button>
          </Card.Actions>
        </Card>
      )
      : <ActivityIndicator />
  );
};

const Images = ({ images }) => {
  const [mutableImages, setImages] = useState(images);

  return (images
    ? images.map((id) => (
      <DebtImage
        key={id}
        id={id}
      />
    ))
    : <></>);
};

const Debts = ({ images, onAdd }) => {
  const { state, dispatch } = useContext(store);
  const [loading, setLoading] = useState(true);

  return (
    <Card style={styles.section} elevation={0}>
      <Card.Title title="Debts" />
      <Images
        images={images}
      />
      <View style={{ flexDirection: 'row', justifyContent: 'space-evenly' }}>
        <Button
          style={styles.addButton}
          onPress={onAdd}
        >
          <Icon name="cash-usd" size={25} />
        </Button>
        <Button
          style={styles.addButton}
          onPress={
          () => {
            ImagePicker.openPicker({
              cropping: true
            })
              .then((i) => uploadImage({ state }, state.event.id, i))
              .then((data) => {
                dispatch({ type: 'ADD_IMAGE_TO_EVENT', imageId: data.image_id });
              })
              .catch((e) => {
                if (e.code === 'E_PICKER_CANCELLED') return;
                throw e;
              });
          }
        }
        >
          <Icon name="image-plus" size={25} />
        </Button>
      </View>
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
